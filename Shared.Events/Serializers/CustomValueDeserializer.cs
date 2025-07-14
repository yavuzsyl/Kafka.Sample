using Confluent.Kafka;
using System.Text.Json;

namespace Shared.Events.Serializers;

public class CustomValueDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return JsonSerializer.Deserialize<T>(data)!;
    }
}
