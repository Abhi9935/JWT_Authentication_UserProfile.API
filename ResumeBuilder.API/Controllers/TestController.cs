using Microsoft.AspNetCore.Mvc;

namespace ResumeBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API Working Successfully");
        }
    }
}