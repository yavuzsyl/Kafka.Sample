using Confluent.Kafka.Admin;
using Confluent.Kafka;

namespace Kafka.Producer;

public class KafkaService
{
    public async Task CreateTopicAsync(string topicName, int partitionsCount)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig()
        { BootstrapServers = "localhost:9094" }).Build();

        try
        {
            await adminClient.CreateTopicsAsync(
            [
                new TopicSpecification(){ Name = topicName, NumPartitions = partitionsCount, ReplicationFactor = 1 }
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
}
