namespace Order.API.Dtos
{
    public record OrderCreateRequest(string UserId, decimal TotalPrice);
}
