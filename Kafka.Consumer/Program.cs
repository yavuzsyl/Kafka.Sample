// See https://aka.ms/new-console-template for more information
using Kafka.Consumer;

Console.WriteLine("Consumer");

var kafkaService = new KafkaService();
//var topicName = "use-case-1-topic";
//await kafkaService.ConsumeMessageWithNullKeyAsync(topicName);

//var topicName = "use-case-2-topic";
//await kafkaService.ConsumeMessageWithIntKeyAsync(topicName);

//var topicName = "use-case-3-topic";
//await kafkaService.ConsumeComplexMessageWithIntKeyAsync(topicName);

//var topicName = "use-case-3-topic-w-h";
//await kafkaService.ConsumeComplexMessageWithIntKeyAndHeaderAsync(topicName);

//var topicName = "use-case-3-topic-w-cmpx-key";
//await kafkaService.ConsumeComplexMessageWithComplexKeyAsync(topicName);

//var topicName = "use-case-4-topic-specific-partition";
//await kafkaService.ConsumeFromSpecificPartitionAsync(topicName, 2);

//var topicName = "use-case-5-topic-specific-partition-offset";
//await kafkaService.ConsumeFromSpecificPartitionOffsetAsync(topicName, 3,27);


var topicName = "message-with-ack";
await kafkaService.ConsumeMessageWithAckAsync(topicName);


Console.ReadLine();
