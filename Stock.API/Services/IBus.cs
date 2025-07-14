using Confluent.Kafka;

namespace Stock.API.Services;

public interface IBus
{
    ConsumerConfig GetConfig(string groupId);
}
