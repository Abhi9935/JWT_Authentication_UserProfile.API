using ResumeBuilder.API.Models.JWT.Models;

namespace ResumeBuilder.API.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);

        Task<RefreshToken?> GetByHashAsync(string tokenHash);

        Task<IEnumerable<RefreshToken>> GetUserTokensAsync(int userId);

        Task<RefreshToken?> GetActiveTokenAsync(int userId, string tokenHash);
        Task RevokeFamilyAsync(Guid tokenFamilyId, string? ipAddress);
        Task RevokeAsync(RefreshToken token);

        Task RevokeAllAsync(int userId); 
        Task<int> RevokeAllForUserAsync(int userId, string? ipAddress);

        Task DeleteExpiredTokensAsync();

        Task SaveChangesAsync();
    }
}