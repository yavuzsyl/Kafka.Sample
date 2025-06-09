using Kafka.Producer;

Console.WriteLine("Producer");

var kafkaService = new KafkaService();

//var topicName = "use-case-1-topic";
var topicName = "use-case-2-topic";
await kafkaService.CreateTopicAsync(topicName, partitionsCount: 3);
await kafkaService.SendMessageWithIntKeyAsync(topicName);