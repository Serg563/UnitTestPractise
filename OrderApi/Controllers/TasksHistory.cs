using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Entities;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksHistory : ControllerBase
    {
        private readonly AppOrderContext _context;
        public TasksHistory(AppOrderContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllTasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            return Ok(await _context.DevTasks.ToListAsync());
        }
        [HttpPost("AddTask")]
        public async Task<IActionResult> AddTask(DevTask task)
        {
            DevTask newTask = new DevTask()
            {
               
                Title = task.Title,
                Description = task.Description,
                Duration = task.Duration,
                Stage = task.Stage,
                isCompleted = task.isCompleted
            };

            _context.DevTasks.Add(newTask);
            await _context.SaveChangesAsync();

            return Ok(newTask);
        }
    }
   
}
