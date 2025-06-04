using Kafka.Producer;

Console.WriteLine("Producer");

var kafkaService = new KafkaService();

var topicName = "use-case-1-topic";
await kafkaService.CreateTopicAsync(topicName, partitionsCount: 3);
await kafkaService.SendMessageWithNullKeyAsync(topicName);