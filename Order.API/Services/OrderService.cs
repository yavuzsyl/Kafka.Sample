using Order.API.Dtos;
using Shared.Events;
using Shared.Events.Events;

namespace Order.API.Services
{
    public class OrderService(IBus bus)
    {
        public async Task<bool> Create(OrderCreateRequest request)
        {
            //db 

            var orderCode = Guid.NewGuid().ToString();
            var orderCreatedEvent = new OrderCreatedEvent(orderCode,
                userId: request.UserId, totalPrice: request.TotalPrice);

            return await bus.Publish(orderCode, orderCreatedEvent, BusConstants.OrderCreatedEventTopicName);
        }
    }
}
