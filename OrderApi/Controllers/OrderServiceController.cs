using Microsoft.AspNetCore.Mvc;
using OrderApi.DTO;
using OrderApi.Entities;
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

        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder([FromBody]AddOrderDTO addOrder)
        {
            await orderService.AddOrder(addOrder);
            return Ok();
        }

        [HttpPut("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder([FromBody]Order updateOrder)
        {
            await orderService.UpdateOrder(updateOrder);
            return NoContent();
        }

        [HttpDelete("DeleteOrderById/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await orderService.DeleteOrder(id);
            return NoContent();
        }

        [HttpGet("GetOrderById/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await orderService.GetOrderById(id);
            return Ok(order);
        }
    }
}
