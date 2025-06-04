// See https://aka.ms/new-console-template for more information
using Kafka.Consumer;

Console.WriteLine("Consumer");

var kafkaService = new KafkaService();
var topicName = "use-case-1-topic";
await kafkaService.ConsumeMessageWithNullKeyAsync(topicName);
Console.ReadLine();