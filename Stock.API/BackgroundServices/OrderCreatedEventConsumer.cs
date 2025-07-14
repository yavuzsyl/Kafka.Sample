using Confluent.Kafka;
using Shared.Events;
using Shared.Events.Events;
using Shared.Events.Serializers;
using Stock.API.Services;

namespace Stock.API.BackgroundServices
{
    public class OrderCreatedEventConsumer(IBus bus) : BackgroundService
    {
        private IConsumer<string, OrderCreatedEvent> consumer;
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            consumer = new ConsumerBuilder<string, OrderCreatedEvent>(bus.GetConfig(BusConstants.OrderCreatedEventTopicGroup))
            .SetValueDeserializer(new CustomValueDeserializer<OrderCreatedEvent>())
            .Build();

            consumer.Subscribe(BusConstants.OrderCreatedEventTopicName);
            return base.StartAsync(cancellationToken);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
    
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(5000);
                if (consumeResult != null)
                {
                    try
                    {
                        var @event = consumeResult.Message.Value;
                        Console.WriteLine($"total price: {@event.totalPrice}, order code: {@event.orderCode}");
                        consumer.Commit(consumeResult);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                await Task.Delay(5, stoppingToken);
            }
        }
    }
}
