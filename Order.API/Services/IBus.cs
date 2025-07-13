namespace Order.API.Services;

public interface IBus
{
    Task<bool> Publish<T1, T2>(T1 key, T2 value, string topicName);
}
