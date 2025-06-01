using Kafka.Producer;

Console.WriteLine("Producer");

await KafkaService.CreateTopicAsync();