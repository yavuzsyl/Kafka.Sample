using Kafka.Producer;

Console.WriteLine("Producer");

var kafkaService = new KafkaService();

//var topicName = "use-case-1-topic";
//await kafkaService.SendMessageWithNullKeyAsync(topicName);

//var topicName = "use-case-2-topic";
//await kafkaService.SendMessageWithIntKeyAsync(topicName);

var topicName = "use-case-3-topic";
await kafkaService.CreateTopicAsync(topicName, partitionsCount: 3);
await kafkaService.SendComplexTypeMessageWithIntKeyAsync(topicName);