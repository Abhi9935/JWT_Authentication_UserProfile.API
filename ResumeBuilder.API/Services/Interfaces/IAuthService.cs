using ResumeBuilder.API.DTOs;

namespace ResumeBuilder.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDTO dto);
        Task<LoginResponseDTO?> LoginAsync(LoginDTO dto, string? ipAddress, string? userAgent);
        Task<LoginResponseDTO?> RefreshTokenAsync(RefreshTokenRequestDTO dto, string? ipAddress, string? userAgent);
        Task<bool> LogoutAsync(LogoutRequestDTO dto, string? ipAddress);
        Task<bool> LogoutAllDevicesAsync(int userId, string? ipAddress);
    }
}
