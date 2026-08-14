using ResumeBuilder.API.Models;
using System.Security.Claims;

namespace ResumeBuilder.API.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);

        string GenerateRefreshToken();

        string HashRefreshToken(string refreshToken);

        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

        bool ValidateAccessToken(string token);
    }
}
