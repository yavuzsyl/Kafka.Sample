using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Kafka.Producer.Events;
using System.Diagnostics;
using System.Text;

namespace Kafka.Producer;

public class KafkaService
{
    public async Task CreateTopicAsync(string topicName, int partitionsCount)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig()
        { BootstrapServers = "localhost:9094" }).Build();

        try
        {
            var config = new Dictionary<string, string>() {
                //https://docs.confluent.io/platform/current/installation/configuration/topic-configs.html
                { "message.timestamp.type", "LogAppendTime" }
            };

            await adminClient.CreateTopicsAsync(
            [
                new TopicSpecification(){
                    Name = topicName,
                    NumPartitions = partitionsCount,
                    ReplicationFactor = 1,
                    Configs = config
                }
            ]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task CreateTopicWithRetentionAsync(string topicName, int partitionsCount)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig()
        { BootstrapServers = "localhost:9094" }).Build();

        try
        {
            TimeSpan oneMonth = TimeSpan.FromDays(30);
            var config = new Dictionary<string, string>() {
                //{ "retention.ms", "-1" } // forever
                {"retention.ms", oneMonth.TotalMilliseconds.ToString() }, // 30 days
            };

            await adminClient.CreateTopicsAsync(
            [
                new TopicSpecification(){
                    Name = topicName,
                    NumPartitions = partitionsCount,
                    ReplicationFactor = 1,
                    Configs = config
                }
            ]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task CreateTopicWithClusterAsync(string topicName, int partitionsCount = 9)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig()
        { BootstrapServers = "localhost:7000,localhost:7001,localhost:7002" }).Build();

        try
        {
            await adminClient.CreateTopicsAsync(
            [
                new TopicSpecification(){
                    Name = topicName,
                    NumPartitions = partitionsCount,
                    ReplicationFactor = 3, // not bigger than partitionsCount
                }
            ]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


    public async Task CreateTopicWithRetryAndClusterAsync(string topicName, int partitionsCount = 9)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig()
        { BootstrapServers = "localhost:7000,localhost:7001,localhost:7002" }).Build();

        try
        {
            var configs = new Dictionary<string, string>
            {
                {"min.insync.replicas","3"} // if ack set as all, min 3 replicas should receive the message
            };

            await adminClient.CreateTopicsAsync(
            [
                new TopicSpecification(){
                    Name = topicName,
                    NumPartitions = partitionsCount,
                    ReplicationFactor = 3,
                    Configs = configs
                }
            ]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


    public async Task SendMessageWithNullKeyAsync(string topicName)
    {
        var config = new ProducerConfig() { BootstrapServers = "localhost:9094" }; //broker

        using var producer = new ProducerBuilder<Null, string>(config).Build(); // primitive types will be serialized by the library

        foreach (var item in Enumerable.Range(1, 30))
        {
            var message = new Message<Null, string>() { Value = $"Message(use-case-1) {item}" };
            var result = await producer.ProduceAsync(topicName, message);

            foreach (var property in result.GetType().GetProperties())
            {
                Console.WriteLine($"{property.Name} : {property.GetValue(result)}");
            }

            Console.WriteLine("----------------------------------------");
            await Task.Delay(150);
        }
    }


    /// <summary>
    /// By setting the Key property, Kafka’s default partitioner hashes that key, so every message with the same key is routed to (and kept in order within) the same partition. Different keys can still spread across partitions.
    /// </summary>
    public async Task SendMessageWithIntKeyAsync(string topicName)
    {
        var config = new ProducerConfig() { BootstrapServers = "localhost:9094" };

        using var producer = new ProducerBuilder<int, string>(config).Build();

        foreach (var item in Enumerable.Range(1, 100))
        {
            var message = new Message<int, string>() { Value = $"Message(use-case-1) {item}", Key = item };  // same key ⇒ same partition
            var result = await producer.ProduceAsync(topicName, message);

            foreach (var property in result.GetType().GetProperties())
            {
                Console.WriteLine($"{property.Name} : {property.GetValue(result)}");
            }

            Console.WriteLine("----------------------------------------");
        }
    }

    public async Task SendComplexTypeMessageWithIntKeyAsync(string topicName)
    {
        var config = new ProducerConfig() { BootstrapServers = "localhost:9094" };

        using var producer = new ProducerBuilder<int, OrderCreatedEvent>(config)
            .SetValueSerializer(new CustomValueSerializer<OrderCreatedEvent>())
            .Build();

        foreach (var item in Enumerable.Range(1, 100))
        {
            var orderCreatedEvent = new OrderCreatedEvent { OrderCode = Guid.NewGuid().ToString(), TotalPrice = item * 100, UserId = item };
            var message = new Message<int, OrderCreatedEvent>() { Value = orderCreatedEvent, Key = item };  // same key ⇒ same partition
            var result = await producer.ProduceAsync(topicName, message);

            foreach (var property in result.GetType().GetProperties())
            {
                Console.WriteLine($"{property.Name} : {property.GetValue(result)}");
            }

            Console.WriteLine("----------------------------------------");
        }
    }


    public async Task SendComplexTypeMessageWithIntKeyAndHeaderAsync(string topicName)
    {
        var config = new ProducerConfig() { BootstrapServers = "localhost:9094" };

        using var producer = new ProducerBuilder<int, OrderCreatedEvent>(config)
            .SetValueSerializer(new CustomValueSerializer<OrderCreatedEvent>())
            .Build();

        foreach (var item in Enumerable.Range(1, 3))
        {
            var orderCreatedEvent = new OrderCreatedEvent { OrderCode = Guid.NewGuid().ToString(), TotalPrice = item * 100, UserId = item };

            var header = new Headers
            {
                { "correlation_id", Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()) },
                { "version", Encoding.UTF8.GetBytes("v1") }
            };

            var message = new Message<int, OrderCreatedEvent>()
            {
                Value = orderCreatedEvent,
                Key = item, // same key ⇒ same partition
                Headers = header
            };
            var result = await producer.ProduceAsync(topicName, message);

            foreach (var property in result.GetType().GetProperties())
            {
                Console.WriteLine($"{property.Name} : {property.GetValue(result)}");
            }

            Console.WriteLine("----------------------------------------");
        }
    }

    public async Task SendComplexTypeMessageWithComplexKeyAsync(string topicName)
    {
        var config = new ProducerConfig() { BootstrapServers = "localhost:9094" };

        using var producer = new ProducerBuilder<MessageKey, OrderCreatedEvent>(config)
            .SetValueSerializer(new CustomValueSerializer<OrderCreatedEvent>())
            .SetKeySerializer(new CustomValueSerializer<MessageKey>())
            .Build();

        foreach (var item in Enumerable.Range(1, 3))
        {
            var orderCreatedEvent = new OrderCreatedEvent { OrderCode = Guid.NewGuid().ToString(), TotalPrice = item * 100, UserId = item };

            var header = new Headers
            {
                { "correlation_id", Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()) },
                { "version", Encoding.UTF8.GetBytes("v1") }
            };

            var message = new Message<MessageKey, OrderCreatedEvent>()
            {
                Value = orderCreatedEvent,
                Key = new MessageKey(Guid.NewGuid().ToString()),
                Headers = header
            };
            var result = await producer.ProduceAsync(topicName, message);

            foreach (var property in result.GetType().GetProperties())
            {
                Console.WriteLine($"{property.Name} : {property.GetValue(result)}");
            }

            Console.WriteLine("----------------------------------------");
        }
    }

    public async Task SendMessageToSpecificPartitionAsync(string topicName, int partition)
    {
        var config = new ProducerConfig() { BootstrapServers = "localhost:9094" };

        using var producer = new ProducerBuilder<MessageKey, OrderCreatedEvent>(config)
            .SetValueSerializer(new CustomValueSerializer<OrderCreatedEvent>())
            .SetKeySerializer(new CustomValueSerializer<MessageKey>())
            .Build();

        foreach (var item in Enumerable.Range(1, 10))
        {
            var orderCreatedEvent = new OrderCreatedEvent { OrderCode = Guid.NewGuid().ToString(), TotalPrice = item * 100, UserId = item };

            var header = new Headers
            {
                { "correlation_id", Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()) },
                { "version", Encoding.UTF8.GetBytes("v1") }
            };

            var message = new Message<MessageKey, OrderCreatedEvent>()
            {
                Value = orderCreatedEvent,
                Key = new MessageKey(Guid.NewGuid().ToString()),
                Headers = header
            };

            var topicPartition = new TopicPartition(topicName, new Partition(partition));

            var result = await producer.ProduceAsync(topicPartition, message);

            foreach (var property in result.GetType().GetProperties())
            {
                Console.WriteLine($"{property.Name} : {property.GetValue(result)}");
            }

            Console.WriteLine("----------------------------------------");
        }
    }

    public async Task SendMessageWithAckAsync(string topicName)
    {
        var config = new ProducerConfig() { BootstrapServers = "localhost:9094", Acks = Acks.Leader };

        using var producer = new ProducerBuilder<Null, string>(config).Build();

        foreach (var item in Enumerable.Range(1, 10))
        {
            var message = new Message<Null, string>() { Value = $"Message(with ack) {item}" };
            var result = await producer.ProduceAsync(topicName, message);

            foreach (var property in result.GetType().GetProperties())
            {
                Console.WriteLine($"{property.Name} : {property.GetValue(result)}");
            }

            Console.WriteLine("----------------------------------------");
            await Task.Delay(150);
        }
    }


    public async Task SendMessageToClusterTopicAsync(string topicName)
    {
        var config = new ProducerConfig() { BootstrapServers = "localhost:7000,localhost:7001,localhost:7002", Acks = Acks.All };

        using var producer = new ProducerBuilder<Null, string>(config).Build();

        foreach (var item in Enumerable.Range(1, 10))
        {
            var message = new Message<Null, string>() { Value = $"Message(from multiple brokers) {item}" };
            var result = await producer.ProduceAsync(topicName, message);

            foreach (var property in result.GetType().GetProperties())
            {
                Console.WriteLine($"{property.Name} : {property.GetValue(result)}");
            }

            Console.WriteLine("----------------------------------------");
            await Task.Delay(150);
        }
    }

    public async Task SendMessageWithRetryToClusterTopicAsync(string topicName)
    {
        var config = new ProducerConfig()
        {
            BootstrapServers = "localhost:7000,localhost:7001,localhost:7002",
            Acks = Acks.All,
            //MessageTimeoutMs = 7000,
            MessageSendMaxRetries = 5,
            RetryBackoffMs = 1000,
            RetryBackoffMaxMs = 2000,
        };
        //Acks.All 
        //Acks.Leader
        //Acks.None
        // MessageSendMaxRetries 
        // MessageTimeoutMs - try untill times up

        _ = Task.Run(async () =>
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(Convert.ToInt32(stopwatch.Elapsed.TotalSeconds));
                Console.Write(timeSpan.ToString("c"));
                Console.Write("\r");

                await Task.Delay(1000);
            }
        });


        using var producer = new ProducerBuilder<Null, string>(config).Build();

        var message = new Message<Null, string>() { Value = $"Message retry" };
        var result = await producer.ProduceAsync(topicName, message);

        foreach (var property in result.GetType().GetProperties())
        {
            Console.WriteLine($"{property.Name} : {property.GetValue(result)}");
        }

        Console.WriteLine("----------------------------------------");
        await Task.Delay(150);
    }
}
