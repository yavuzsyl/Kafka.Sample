// See https://aka.ms/new-console-template for more information
using Kafka.ConsumerMultipleGroupTest;

Console.WriteLine("Consumer 2");

var kafkaService = new KafkaService();
var topicName = "use-case-1-topic";
await kafkaService.ConsumeMessageWithNullKeyAsync(topicName);
Console.ReadLine();