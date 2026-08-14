using Microsoft.Extensions.Options;
using ResumeBuilder.API.DTOs;
using ResumeBuilder.API.Model.Users;
using ResumeBuilder.API.Models;
using ResumeBuilder.API.Models.JWT.Models;
using ResumeBuilder.API.Repositories.Interfaces;
using ResumeBuilder.API.Services.Interfaces;
using System.Runtime.Intrinsics.Arm;

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

            // Step 1 : Find User           
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
                return null;

            // Step 2 : Verify Password 
            bool passwordVerified = BCrypt.Net.BCrypt.Verify(dto.Password, user.UserHashedPass);

            if (!passwordVerified)
                return null;
            
            // Step 3 : Check Account Status
            if (!user.AccountStatus.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Account is inactive.");
            }
            
            // Step 4 : Generate Access Token
            string accessToken = _jwtService.GenerateAccessToken(user);
                        
            // Step 5 : Generate Refresh Token
            string refreshToken = _jwtService.GenerateRefreshToken();
                        
            // Step 6 : Hash Refresh Token
            string refreshTokenHash = _jwtService.HashRefreshToken(refreshToken);
            
            // Step 7 : Create RefreshToken Entity
            RefreshToken refreshTokenEntity =  new RefreshToken
                {
                    UserId = user.UserId,
                    TokenHash = refreshTokenHash,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),

                    RevokedAt = null,
                    ReplacedByTokenHash = null,
                    CreatedByIp = ipAddress,
                    RevokedByIp = null,
                    UserAgent = userAgent
                };

            
            // Step 8 : Save Refresh Token
            await _refreshTokenRepository.AddAsync(refreshTokenEntity);

            await _refreshTokenRepository.SaveChangesAsync();
            
            // Step 9 : Return Tokens
            
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
            // Step 4 – Read Expired JWT
            var principal = _jwtService.GetPrincipalFromExpiredToken(dto.AccessToken);

            if (principal == null)
                return null;

            //Step 5 - Get UserId
            var userIdClaim = principal.FindFirst("UserId");

            if (userIdClaim == null)
                return null;

            int userId = int.Parse(userIdClaim.Value);
            
            //Step 6 – Hash Incoming Refresh Token
            string tokenHash =_jwtService.HashRefreshToken(dto.RefreshToken);

            // Step 7 - Find The Token ; Repository verifies: UserId, Hash, Not Revoked,Not Expired
            var existingToken = await _refreshTokenRepository.GetActiveTokenAsync(userId, tokenHash);

            if (existingToken == null)
                return null;

            //Step 8 – Load User
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return null;

            // Step 9 – Generate New Access Token]

            string accessToken = _jwtService.GenerateAccessToken(user);

            // Step 10 – Generate New Refresh Token
            string refreshToken = _jwtService.GenerateRefreshToken();

            // Step 11 – Hash New Token
            string newHash = _jwtService.HashRefreshToken(refreshToken);
            
            // Step 12 – Revoke Old Token
            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.RevokedByIp = ipAddress;
            existingToken.ReplacedByTokenHash = newHash;
            
            // Step 13 – Save New Refresh Token
            RefreshToken newEntity = new RefreshToken
            {
                UserId = userId,
                TokenHash = newHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),                
                CreatedByIp = ipAddress,
                UserAgent = userAgent
            };

            await _refreshTokenRepository.AddAsync(newEntity);
            await _refreshTokenRepository.SaveChangesAsync();

            // Step 14 – Return New Tokens
            return new LoginResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes),
                RefreshTokenExpiresAt = newEntity.ExpiresAt,
                TokenType = "Bearer"
            };

        }

        public async Task<bool> LogoutAsync(LogoutRequestDTO dto, string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(dto.RefreshToken))
            {
                return false;
            }

            // Step 1 - Hash the supplied refresh token
            string tokenHash = _jwtService.HashRefreshToken(dto.RefreshToken);

            // Step 2 - Find the token
            var refreshToken = await _refreshTokenRepository.GetByHashAsync(tokenHash);

            // Step 3 - Token doesn't exist
            if (refreshToken == null)
            {
                return false;
            }

            // Step 4 - Check whether it is already revoked
            if (refreshToken.RevokedAt.HasValue)
            {
                return false;
            }

            // Step 5 - Revoke token
            refreshToken.RevokedAt = DateTime.UtcNow;

            refreshToken.RevokedByIp =  ipAddress;

            // Step 6 - Save changes
            await _refreshTokenRepository.SaveChangesAsync();

            return true;
        }
    }
}