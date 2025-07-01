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

//var topicName = "use-case-3-topic-w-cmpx-key";
//await kafkaService.SendComplexTypeMessageWithComplexKeyAsync(topicName);

//var topicName = "use-case-4-topic-specific-partition";
//await kafkaService.SendMessageToSpecificPartitionAsync(topicName, 2);

//var topicName = "use-case-5-topic-specific-partition-offset";
//await kafkaService.SendMessageToSpecificPartitionAsync(topicName, 3);


// Producer acknowledgements (acks)
//
// acks = "all"  or -1   → Broker waits until the leader **and every in-sync replica**
//                          confirm the write.  Highest durability, highest latency.
//
// acks = "1"             → Broker returns as soon as the **leader partition** has
//                          written the record.  Good durability–latency trade-off.
//
// acks = "0"             → Fire-and-forget: the producer does not wait for any
//                          acknowledgement.  Lowest latency, but **no delivery
//                          guarantee** (lost records are possible).

//var topicName = "message-with-ack";
//await kafkaService.CreateTopicAsync(topicName, partitionsCount: 5);
//await kafkaService.SendMessageWithAckAsync(topicName);

var topicName = "retention-topic-2";
await kafkaService.CreateTopicWithRetentionAsync(topicName, partitionsCount: 2);