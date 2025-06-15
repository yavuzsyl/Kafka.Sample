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

var topicName = "use-case-3-topic-w-h";
await kafkaService.ConsumeComplexMessageWithIntKeyAndHeaderAsync(topicName);

Console.ReadLine();