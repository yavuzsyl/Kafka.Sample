using Confluent.Kafka;
using System.Text;
using System.Text.Json;

namespace Kafka.Producer;

internal class CustomValueSerializer<T> : ISerializer<T>
{
    public byte[] Serialize(T data, SerializationContext context) 
        => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
}
