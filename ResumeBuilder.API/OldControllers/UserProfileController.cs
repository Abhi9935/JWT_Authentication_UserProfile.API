using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.API.Services;

namespace ResumeBuilder.API.Controllers
{
    [ApiController] // 🟢 Tells OpenAPI this is an API layout, not a MVC web view
    [Route("api/[controller]")] // 🟢 Explicit routing anchor

    public class UserProfileController : Controller
    {
        // GET: UserProfile
        [HttpGet] // 🟢 Ensure EVERY endpoint has a method verb assigned (HttpGet, HttpPost, etc.)
        public IActionResult Get()
        {
            return Ok();
        }
        private readonly IResumebuilderServices _resumebuilderServices;

        public UserProfileController(IResumebuilderServices service)
        {
            _resumebuilderServices = service;
        }

    }
}
