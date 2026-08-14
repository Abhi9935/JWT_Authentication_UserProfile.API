using System.Security.Claims;

namespace ResumeBuilder.API.Helpers
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _context;

        public CurrentUserService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public bool IsAuthenticated =>
            _context.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public int UserId =>
            int.Parse(_context.HttpContext?.User?.FindFirst("UserId")?.Value ?? "0");

        public string Username =>
            _context.HttpContext?.User?.Identity?.Name ?? "";

        public string Role =>
            _context.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? "";
    }
}