using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Entities;
using System.Text.Json;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        //added come comments
        private readonly AppOrderContext _context;
        public TagController(AppOrderContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetAllTags()
        {
            var res = await _context.Tags.ToListAsync();
            return Ok(res);
            //string jsonArrayFromDatabase = "[\"First\",\"Second\",\"Third\"]";

            //var tag = new Tag
            //{
            //    Id = 123,
            //    Name = "Example",
            //    ApplicableTo = jsonArrayFromDatabase
            //};
            //var json = JsonSerializer.Serialize(tag);

            //return Ok(json);
        }
        [HttpGet("GetJsonAsync")]
        public async Task<IActionResult> GetJsonAsync()
        {
            string jsonArrayFromDatabase = "[\"First\",\"Second\",\"Third\"]";

            return Ok(jsonArrayFromDatabase);
        }
    }
}
