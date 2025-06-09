using Confluent.Kafka;

namespace Kafka.ConsumerMultipleGroupTest;
internal class KafkaService
{
    public async Task ConsumeMessageWithNullKeyAsync(string topicName)
    {
        var config = new ConsumerConfig()
        {
            BootstrapServers = "localhost:9094",
            GroupId = "use-case-1-group-2",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        var consumer = new ConsumerBuilder<Null, string>(config).Build();
        consumer.Subscribe(topicName);

        while (true)
        {
            var consumeResult = consumer.Consume(5000);
            if (consumeResult != null)
                Console.WriteLine($"consumed message: {consumeResult.Message.Value}");
            await Task.Delay(500);
        }
    }
}
