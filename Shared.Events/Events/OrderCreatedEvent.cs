namespace Shared.Events.Events;

public record OrderCreatedEvent(string orderCode, string userId, decimal totalPrice);
