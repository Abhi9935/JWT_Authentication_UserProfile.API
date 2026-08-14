using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.API.Services;

namespace ResumeBuilder.API.Controllers
{
    [ApiController] // 🟢 Tells OpenAPI this is an API layout, not a MVC web view
    [Route("api/[controller]")] // 🟢 Explicit routing anchor

    public class UserController : Controller
    {

        private readonly IResumebuilderServices _resumebuilderServices;

        [HttpGet] // 🟢 Ensure EVERY endpoint has a method verb assigned (HttpGet, HttpPost, etc.)
        public IActionResult Get()
        {
            return Ok();
        }

        public UserController(IResumebuilderServices service)
        {
            _resumebuilderServices = service;
        }
        [HttpGet]
        public IActionResult GetUserDetails(int id)
        {
            return Ok(_resumebuilderServices.GetUsersAllDetails(id));
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            var user = _resumebuilderServices.GetUserByID(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
