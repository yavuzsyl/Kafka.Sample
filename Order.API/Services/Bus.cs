﻿using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Shared.Events.Serializers;

namespace Order.API.Services;

public class Bus(IConfiguration config) : IBus
{
    public readonly ProducerConfig _producerConfig = new()
    {
        BootstrapServers = config.GetSection("BusSettings").GetSection("Kafka")["BootstrapServers"],
        Acks = Acks.All,
        MessageTimeoutMs = 6000,
        AllowAutoCreateTopics = true,
    };

    public async Task<bool> Publish<T1, T2>(T1 key, T2 value, string topicName)
    {
        using var producer = new ProducerBuilder<T1, T2>(_producerConfig)
            .SetKeySerializer(new CustomValueSerializer<T1>())
            .SetValueSerializer(new CustomValueSerializer<T2>())
            .Build();

        var message = new Message<T1, T2>()
        {
            Key = key,
            Value = value,
        };

        var result = await producer.ProduceAsync(topicName, message);

        return result.Status == PersistenceStatus.Persisted;
    }

    public Task CreateTopic(List<string> topicNames)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig()
        {
            BootstrapServers = config.GetSection("BusSettings").GetSection("Kafka")["BootstrapServers"]
        }).Build();

        topicNames.ForEach(async (topicName) =>
        {
            try
            {
                await adminClient.CreateTopicsAsync(
                [
                    new TopicSpecification()
                    {
                      Name = topicName, NumPartitions = 6, ReplicationFactor = 1
                    }
                ]);

                Console.WriteLine("topic created");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });

        return Task.CompletedTask;
    }
}
