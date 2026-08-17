using Microsoft.EntityFrameworkCore;
using ResumeBuilder.API.Data;
using ResumeBuilder.API.Models.JWT.Models;
using ResumeBuilder.API.Repositories.Interfaces;

namespace ResumeBuilder.API.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
        }

        public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.TokenHash == tokenHash);
        }

        public async Task<IEnumerable<RefreshToken>> GetUserTokensAsync(int userId)
        {
            return await _context.RefreshTokens
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<RefreshToken?> GetActiveTokenAsync(int userId, string tokenHash)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.TokenHash == tokenHash &&
                    x.RevokedAt == null &&
                    x.ExpiresAt > DateTime.UtcNow);
        }

        public Task RevokeAsync(RefreshToken token)
        {
            token.RevokedAt = DateTime.UtcNow;
            _context.RefreshTokens.Update(token);

            return Task.CompletedTask;
        }

        public async Task RevokeAllAsync(int userId)
        {
            var tokens =
                await _context.RefreshTokens
                    .Where(x =>
                        x.UserId == userId &&
                        x.RevokedAt == null)
                    .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }
        }

        public async Task<int> RevokeAllForUserAsync(int userId, string? ipAddress)
        {
            var tokens =
                await _context.RefreshTokens
                    .Where(x =>
                        x.UserId == userId &&
                        x.RevokedAt == null &&
                        x.ExpiresAt > DateTime.UtcNow)
                    .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
            }

            return tokens.Count;
        }

        public async Task RevokeFamilyAsync(Guid tokenFamilyId, string? ipAddress)
        {
            var tokens =
                await _context.RefreshTokens
                    .Where(x =>
                        x.TokenFamilyId == tokenFamilyId &&
                        x.RevokedAt == null)
                    .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
            }
        }
        public async Task DeleteExpiredTokensAsync()
        {
            var expired =
                await _context.RefreshTokens
                    .Where(x =>
                        x.ExpiresAt < DateTime.UtcNow)
                    .ToListAsync();

            if (expired.Any())
            {
                _context.RefreshTokens.RemoveRange(expired);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}