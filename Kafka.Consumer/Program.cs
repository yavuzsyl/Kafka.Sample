// See https://aka.ms/new-console-template for more information
using Kafka.Consumer;

Console.WriteLine("Consumer");

var kafkaService = new KafkaService();
//var topicName = "use-case-1-topic";
var topicName = "use-case-2-topic";
await kafkaService.ConsumeMessageWithIntKeyAsync(topicName);
Console.ReadLine();