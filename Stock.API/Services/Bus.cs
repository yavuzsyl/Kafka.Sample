using Confluent.Kafka;

namespace Stock.API.Services;

public class Bus(IConfiguration config) : IBus
{
    public ConsumerConfig GetConfig(string groupId)
    {
        return new()
        {
            BootstrapServers = config.GetSection("BusSettings").GetSection("Kafka")["BootstrapServers"],
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };
    }
}
