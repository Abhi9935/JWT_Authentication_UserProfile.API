using ResumeBuilder.API.Models.JWT.Models;

namespace ResumeBuilder.API.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);

        Task<RefreshToken?> GetByHashAsync(string tokenHash);

        Task<IEnumerable<RefreshToken>> GetUserTokensAsync(int userId);

        Task<RefreshToken?> GetActiveTokenAsync(int userId, string tokenHash);

        Task RevokeAsync(RefreshToken token);

        Task RevokeAllAsync(int userId);

        Task DeleteExpiredTokensAsync();

        Task SaveChangesAsync();
    }
}