using Confluent.Kafka;
using System.Text;
using System.Text.Json;

namespace Shared.Events;

public class CustomValueSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context)
        => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
}
