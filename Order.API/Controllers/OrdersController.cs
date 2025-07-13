using Microsoft.AspNetCore.Mvc;
using Order.API.Dtos;
using Order.API.Services;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(OrderService service) : ControllerBase
    {
        public async Task<IActionResult> Create(OrderCreateRequest request)
        {
            return Ok(await service.Create(request));
        }
    }
}
