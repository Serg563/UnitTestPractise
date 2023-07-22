using Microsoft.AspNetCore.Mvc;
using OrderApi.Models;
using OrderApi.Services;
using System.Collections;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderServiceController : ControllerBase
    {
        private readonly IOrderService orderService;
        public OrderServiceController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        [HttpGet("GetAllOrders")]
        public async Task<ActionResult<IEnumerable<OrderGetAllModel>>> GetAllOrder()
        {
            return Ok(await orderService.GetAllOrders());
        }
    }
}
