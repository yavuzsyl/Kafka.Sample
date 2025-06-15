using Kafka.Producer;

Console.WriteLine("Producer");

var kafkaService = new KafkaService();

//var topicName = "use-case-1-topic";
//await kafkaService.SendMessageWithNullKeyAsync(topicName);

//var topicName = "use-case-2-topic";
//await kafkaService.SendMessageWithIntKeyAsync(topicName);

//var topicName = "use-case-3-topic";
//await kafkaService.SendComplexTypeMessageWithIntKeyAsync(topicName);

//var topicName = "use-case-3-topic-w-h";
//await kafkaService.SendComplexTypeMessageWithIntKeyAndHeaderAsync(topicName);


var topicName = "use-case-3-topic-w-cmpx-key";
await kafkaService.CreateTopicAsync(topicName, partitionsCount: 3);
await kafkaService.SendComplexTypeMessageWithComplexKeyAsync(topicName);