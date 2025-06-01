using Confluent.Kafka.Admin;
using Confluent.Kafka;

namespace Kafka.Producer;

public static class KafkaService
{
    static string TopicName = "topic_one";

    public static async Task CreateTopicAsync()
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig()
        { BootstrapServers = "localhost:9094" }).Build();

        try
        {
            await adminClient.CreateTopicsAsync(
            [
                new TopicSpecification(){ Name = TopicName, NumPartitions = 3, ReplicationFactor = 1 }
            ]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
