using Confluent.Kafka;
using Kafka.Consumer.Events;
using System.Text;

namespace Kafka.Consumer;

internal class KafkaService
{
    public async Task ConsumeMessageWithNullKeyAsync(string topicName)
    {
        var config = new ConsumerConfig()
        {
            BootstrapServers = "localhost:9094",
            GroupId = "use-case-1-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest // if no committed offset is found, begin at **offset 0** and replay the entire topic history.
                                                       // AutoOffsetReset.Latest  – if no committed offset is found, jump to the **end of the log** and receive **only messages published after** the consumer starts. 
        };

        var consumer = new ConsumerBuilder<Null, string>(config).Build();
        consumer.Subscribe(topicName); // kafka-pull, rabbitmq push

        while (true)
        {
            /*
             * Blocks until a message is available. Internally polls the broker, buffers
             * fetched records in a local queue, and returns the next message from that
             * queue as a ConsumeResult<TKey, TValue>.
             *
             * If a timeout is specified, waits until a message arrives or the timeout
             * elapses.
             *
             * Best practice: match the number of consumer instances to the number of
             * partitions. Any consumer beyond the partition count will stay idle because
             * a partition can be assigned to only one consumer in the same group.
             */
            var consumeResult = consumer.Consume(5000);
            if (consumeResult != null)
                Console.WriteLine($"consumed message: {consumeResult.Message.Value}");
            await Task.Delay(500);
        }
    }

    public async Task ConsumeMessageWithIntKeyAsync(string topicName)
    {
        var config = new ConsumerConfig()
        {
            BootstrapServers = "localhost:9094",
            GroupId = "use-case-2-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var consumer = new ConsumerBuilder<int, string>(config).Build();
        consumer.Subscribe(topicName);

        while (true)
        {
            var consumeResult = consumer.Consume(5000);
            if (consumeResult != null)
                Console.WriteLine($"consumed message: Key: {consumeResult.Message.Key} - Value: {consumeResult.Message.Value}");

            await Task.Delay(1);
        }
    }

    public async Task ConsumeComplexMessageWithIntKeyAsync(string topicName)
    {
        var config = new ConsumerConfig()
        {
            BootstrapServers = "localhost:9094",
            GroupId = "use-case-3-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var consumer = new ConsumerBuilder<int, OrderCreatedEvent>(config)
            .SetValueDeserializer(new CustomValueDeserializer<OrderCreatedEvent>())
            .Build();
        consumer.Subscribe(topicName);

        while (true)
        {
            var consumeResult = consumer.Consume(5000);
            if (consumeResult != null)
                Console.WriteLine($"consumed message: Key: {consumeResult.Message.Key} - Value: UserId:{consumeResult.Message.Value.UserId}-OrderCode:{consumeResult.Message.Value.OrderCode}-TotalPrice:{consumeResult.Message.Value.TotalPrice}");

            await Task.Delay(5);
        }
    }

    public async Task ConsumeComplexMessageWithIntKeyAndHeaderAsync(string topicName)
    {
        var config = new ConsumerConfig()
        {
            BootstrapServers = "localhost:9094",
            GroupId = "use-case-3-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var consumer = new ConsumerBuilder<int, OrderCreatedEvent>(config)
            .SetValueDeserializer(new CustomValueDeserializer<OrderCreatedEvent>())
            .Build();
        consumer.Subscribe(topicName);

        while (true)
        {
            var consumeResult = consumer.Consume(5000);
            if (consumeResult != null)
            {
                var correlationId = Encoding.UTF8.GetString(consumeResult.Message.Headers.GetLastBytes("correlation_id"));
                var version = Encoding.UTF8.GetString(consumeResult.Message.Headers.GetLastBytes("version"));

                if (consumeResult != null)
                    Console.WriteLine(
                        $"consumed message: Key: {consumeResult.Message.Key} " +
                        $"- Value: UserId:{consumeResult.Message.Value.UserId}" +
                        $"- OrderCode:{consumeResult.Message.Value.OrderCode}" +
                        $"- TotalPrice:{consumeResult.Message.Value.TotalPrice}" +
                        $"- Header-Correlation-id: {correlationId}" +
                        $"- version: {version}");
                await Task.Delay(5);
            }
        }
    }

    public async Task ConsumeComplexMessageWithComplexKeyAsync(string topicName)
    {
        var config = new ConsumerConfig()
        {
            BootstrapServers = "localhost:9094",
            GroupId = "use-case-3-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var consumer = new ConsumerBuilder<MessageKey, OrderCreatedEvent>(config)
            .SetValueDeserializer(new CustomValueDeserializer<OrderCreatedEvent>())
            .SetKeyDeserializer(new CustomValueDeserializer<MessageKey>())
            .Build();
        consumer.Subscribe(topicName);

        while (true)
        {
            var consumeResult = consumer.Consume(5000);
            if (consumeResult != null)
            {
                Console.WriteLine($"Message timestamp :{consumeResult.Message.Timestamp.UtcDateTime}");
                Console.WriteLine($"consumed message: Key: {consumeResult.Message.Key.KeyValue} - Value: UserId:{consumeResult.Message.Value.UserId}-OrderCode:{consumeResult.Message.Value.OrderCode}-TotalPrice:{consumeResult.Message.Value.TotalPrice}");
            }

            await Task.Delay(5);
        }
    }

    public async Task ConsumeFromSpecificPartitionAsync(string topicName, int partition)
    {
        var config = new ConsumerConfig()
        {
            BootstrapServers = "localhost:9094",
            GroupId = "use-case-4-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var consumer = new ConsumerBuilder<MessageKey, OrderCreatedEvent>(config)
            .SetValueDeserializer(new CustomValueDeserializer<OrderCreatedEvent>())
            .SetKeyDeserializer(new CustomValueDeserializer<MessageKey>())
            .Build();

        consumer.Assign(new TopicPartition(topicName, partition));

        while (true)
        {
            var consumeResult = consumer.Consume(5000);
            if (consumeResult != null)
            {
                Console.WriteLine($"Message timestamp :{consumeResult.Message.Timestamp.UtcDateTime}");
                Console.WriteLine($"consumed message: Key: {consumeResult.Message.Key.KeyValue} - Value: UserId:{consumeResult.Message.Value.UserId}-OrderCode:{consumeResult.Message.Value.OrderCode}-TotalPrice:{consumeResult.Message.Value.TotalPrice}");
            }

            await Task.Delay(5);
        }
    }

    public async Task ConsumeFromSpecificPartitionOffsetAsync(string topicName, int partition, int consumeAfterOffset)
    {
        var config = new ConsumerConfig()
        {
            BootstrapServers = "localhost:9094",
            GroupId = "use-case-5-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var consumer = new ConsumerBuilder<MessageKey, OrderCreatedEvent>(config)
            .SetValueDeserializer(new CustomValueDeserializer<OrderCreatedEvent>())
            .SetKeyDeserializer(new CustomValueDeserializer<MessageKey>())
            .Build();

        consumer.Assign(new TopicPartitionOffset(topicName, partition, consumeAfterOffset));

        while (true)
        {
            var consumeResult = consumer.Consume(5000);
            if (consumeResult != null)
            {
                Console.WriteLine($"Message timestamp :{consumeResult.Message.Timestamp.UtcDateTime}");
                Console.WriteLine($"consumed message: Key: {consumeResult.Message.Key.KeyValue} - Value: UserId:{consumeResult.Message.Value.UserId}-OrderCode:{consumeResult.Message.Value.OrderCode}-TotalPrice:{consumeResult.Message.Value.TotalPrice}");
            }

            await Task.Delay(5);
        }
    }

    // ─────────────── Consumer “acknowledgement” in Kafka ───────────────
    //
    // In Kafka, an ACK = committing the **offset**.  
    // • EnableAutoCommit = true   → the client periodically commits offsets in the
    //   background right after the record is fetched (fire-and-forget style).  
    //   Fast, but you may mark a message “done” before your code has actually
    //   processed it.
    //
    // • EnableAutoCommit = false  → you call Commit() explicitly **after** your
    //   business logic succeeds.  Safer (at-least-once), but you must handle commits
    //   and retries yourself.
    //
    public async Task ConsumeMessageWithAckAsync(string topicName)
    {
        var config = new ConsumerConfig()
        {
            BootstrapServers = "localhost:9094",
            GroupId = "group-6",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,   // ← manual ACK mode
            // EnableAutoCommit = true    // ← uncomment for auto-commit mode
        };

        var consumer = new ConsumerBuilder<Null, string>(config).Build();
        consumer.Subscribe(topicName);

        while (true)
        {
            var consumeResult = consumer.Consume(5000);
            if (consumeResult != null)
            {
                try
                {
                    Console.WriteLine($"consumed message: {consumeResult.Message.Value}");

                    // Manual ACK: mark this offset as processed so the
                    // group will resume from the next record on restart
                    consumer.Commit(consumeResult);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Optionally seek back or let the lack of commit trigger a retry
                    throw;
                }
            }

            await Task.Delay(500);
        }
    }

    public async Task ConsumeMessageFromClusterAsync(string topicName)
    {
        var config = new ConsumerConfig()
        {
            BootstrapServers = "localhost:7000,localhost:7001,localhost:7002",
            GroupId = "group-cluster",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        var consumer = new ConsumerBuilder<Null, string>(config).Build();
        consumer.Subscribe(topicName);

        while (true)
        {
            var consumeResult = consumer.Consume(5000);
            if (consumeResult != null)
            {
                try
                {
                    Console.WriteLine($"consumed message: {consumeResult.Message.Value}");
                    consumer.Commit(consumeResult);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            await Task.Delay(500);
        }
    }
}
