using Microsoft.Extensions.Options;
using ResumeBuilder.API.DTOs;
using ResumeBuilder.API.Models;
using ResumeBuilder.API.Models.JWT.Models;
using ResumeBuilder.API.Repositories.Interfaces;
using ResumeBuilder.API.Services.Interfaces;
using System.Security;

namespace ResumeBuilder.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        private readonly IRefreshTokenRepository _refreshTokenRepository;

        private readonly IJwtService _jwtService;

        //private readonly IOptions<JwtSettings> _jwtSettings;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshRepository, IJwtService jwtService, IOptions<JwtSettings> jwtOptions)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshRepository;
            _jwtService = jwtService;
            _jwtSettings = jwtOptions.Value;
        }

        #region Register

        public async Task<bool> RegisterAsync(RegisterDTO dto)
        {
            // Check whether email already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.UserEmail);

            if (existingUser != null)
                return false;

            // Hash Password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            User user = new User
            {
                Username = dto.Username,
                UserEmail = dto.UserEmail.Trim().ToLower(),
                UserType = dto.UserType,
                UserHashedPass = passwordHash,
                AccountStatus = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            await _userRepository.SaveAsync();

            return true;
        }

        #endregion

        #region Login

        public async Task<LoginResponseDTO?> LoginAsync(LoginDTO dto, string? ipAddress, string? userAgent)
        {
            // Normalize Email
            string email = dto.UserEmail.Trim().ToLower();

            //  Find User           
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
                return null;

            //Verify Password 
            bool passwordVerified = BCrypt.Net.BCrypt.Verify(dto.Password, user.UserHashedPass);

            if (!passwordVerified)
                return null;
            
            // Check Account Status
            if (!user.AccountStatus.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is inactive.");
            }
            
            // Generate Access Token
            string accessToken = _jwtService.GenerateAccessToken(user);
                        
            // Generate Refresh Token
            string refreshToken = _jwtService.GenerateRefreshToken();
                        
            // Hash Refresh Token
            string refreshTokenHash = _jwtService.HashRefreshToken(refreshToken);

            Guid tokenFamilyId = Guid.NewGuid();
            // Create RefreshToken Entity
            RefreshToken refreshTokenEntity =  new RefreshToken
                {
                    UserId = user.UserId,
                    TokenHash = refreshTokenHash,
                    TokenFamilyId = tokenFamilyId,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),

                    RevokedAt = null,
                    ReplacedByTokenHash = null,
                    CreatedByIp = ipAddress,
                    RevokedByIp = null,
                    UserAgent = userAgent
                };

            
            // Save Refresh Token
            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            await _refreshTokenRepository.SaveChangesAsync();
            
            // Return Tokens
            
            return new LoginResponseDTO
            {
                AccessToken = accessToken,

                RefreshToken = refreshToken,

                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes),

                RefreshTokenExpiresAt = refreshTokenEntity.ExpiresAt,

                TokenType = "Bearer"
            };
        }

        #endregion

        public async Task<LoginResponseDTO?> RefreshTokenAsync(RefreshTokenRequestDTO dto, string? ipAddress, string? userAgent)
        {
            // Read Expired JWT
            var principal = _jwtService.GetPrincipalFromExpiredToken(dto.AccessToken);

            if (principal == null)
                return null;

            //Get UserId
            var userIdClaim = principal.FindFirst("UserId");

            if (userIdClaim == null)
                return null;

            int userId = int.Parse(userIdClaim.Value);
            
            // Hash Incoming Refresh Token
            string tokenHash =_jwtService.HashRefreshToken(dto.RefreshToken);

            // Find The Token ; Repository verifies: UserId, Hash, Not Revoked,Not Expired
            var existingToken = await _refreshTokenRepository.GetActiveTokenAsync(userId, tokenHash);

            if (existingToken == null)
                return null;

            if (existingToken.UserId != userId)
            {
                return null;
            }

            // REPLAY DETECTION
            if (existingToken.RevokedAt.HasValue)
            {
                // SECURITY EVENT
                await _refreshTokenRepository.RevokeFamilyAsync(existingToken.TokenFamilyId, ipAddress);
                await _refreshTokenRepository.SaveChangesAsync();

                // Do NOT issue new tokens
                throw new SecurityException("Refresh token replay detected."); // need to change : avoid throw Exception for expected authentication outcomes. use enum
            }

            if (existingToken.ExpiresAt <= DateTime.UtcNow)
            {
                return null;
            }
            // Load User
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return null;

            if (!user.AccountStatus.Equals("Active",StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Generate New Access Token
            string accessToken = _jwtService.GenerateAccessToken(user);

            // Generate New Refresh Token
            string refreshToken = _jwtService.GenerateRefreshToken();

            // Hash New Token
            string newHash = _jwtService.HashRefreshToken(refreshToken);
            
            // Revoke Old Token
            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.RevokedByIp = ipAddress;
            existingToken.ReplacedByTokenHash = newHash;
            
            // Save New Refresh Token
            RefreshToken newEntity = new RefreshToken
            {
                UserId = userId,
                TokenHash = newHash,
                TokenFamilyId = existingToken.TokenFamilyId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),                
                CreatedByIp = ipAddress,
                UserAgent = userAgent
            };

            await _refreshTokenRepository.AddAsync(newEntity);
            await _refreshTokenRepository.SaveChangesAsync();

            // Return New Tokens
            return new LoginResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes),
                RefreshTokenExpiresAt = newEntity.ExpiresAt,
                TokenType = "Bearer"
            };

        }

        #region LogOut
        public async Task<bool> LogoutAsync(LogoutRequestDTO dto, string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            {
                return false;
            }

            // Hash the supplied refresh token
            string tokenHash = _jwtService.HashRefreshToken(dto.RefreshToken);

            // Find the token
            var refreshToken = await _refreshTokenRepository.GetByHashAsync(tokenHash);

            // Token doesn't exist
            if (refreshToken == null)
            {
                return false;
            }

            // Check whether it is already revoked
            if (refreshToken.RevokedAt.HasValue)
            {
                return false;
            }

            // Revoke token
            refreshToken.RevokedAt = DateTime.UtcNow;

            refreshToken.RevokedByIp =  ipAddress;

            // Save changes
            await _refreshTokenRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> LogoutAllDevicesAsync(int userId, string? ipAddress)
        {
            if (userId <= 0)
            {
                return false;
            }

            await _refreshTokenRepository.RevokeAllForUserAsync(userId, ipAddress);

            await _refreshTokenRepository.SaveChangesAsync();

            return true;
        }

        #endregion
    }
}