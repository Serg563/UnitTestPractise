using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.DTO;
using OrderApi.Entities;
using OrderApi.Filters;
using OrderApi.Repositories;
using System.Collections;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository repo;
        public OrderController(IOrderRepository repo)
        {
            this.repo = repo;
        }
        [ResourceFilter]
        [CustomActionFilter]
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await repo.GetAllOrders();
            return Ok(orders);
        }

        [HttpGet("GetOrdersById/{id}")]
        public async Task<IActionResult> GetOrdersById(int id)
        {
            var orders = await repo.GetOrderById(id);
            return Ok(orders);
        }

        [HttpDelete("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await repo.DeleteOrder(id);
            return NoContent();
        }

        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder([FromBody]AddOrderDTO order)
        {
            await repo.AddOrder(order);
            return Ok();
        }
    }
}
