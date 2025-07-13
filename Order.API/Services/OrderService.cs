using Order.API.Dtos;
using Shared.Events;

namespace Order.API.Services
{
    public class OrderService(IBus bus)
    {
        public async Task<bool> Create(OrderCreateRequest request)
        {
            //db 

            var orderCode = Guid.NewGuid().ToString();
            var orderCreatedEvent = new OrderCreatedEvent(orderCode,
                userId: "1", totalPrice: 100);

            return await bus.Publish(orderCode, orderCreatedEvent, BusConstants.OrderCreatedEventTopicName);
        }
    }
}
