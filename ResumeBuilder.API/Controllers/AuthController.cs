using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.API.DTOs;
using ResumeBuilder.API.Models;
using ResumeBuilder.API.Repositories.Interfaces;
using ResumeBuilder.API.Services;
using ResumeBuilder.API.Services.Interfaces;
using System.Security;

namespace ResumeBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IAuthService _authService;

        public AuthController(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check duplicate email
            if (await _userRepository.EmailExistsAsync(model.UserEmail))
            {
                return BadRequest(new AuthResponseDTO
                {
                    Success = false,
                    Message = "Email already exists."
                });
            }

            var user = new User
            {
                Username = model.Username,
                UserType = model.UserType,
                UserEmail = model.UserEmail,

                // BCrypt Password Hash
                UserHashedPass = BCrypt.Net.BCrypt.HashPassword(model.Password),

                AccountStatus = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            var userDTO = new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                UserEmail = user.UserEmail,
                UserType = user.UserType,
                AccountStatus = user.AccountStatus
            };

            return Ok(new AuthResponseDTO
            {
                Success = true,
                Message = "Registration successful.",
                User = userDTO
            });
        }

        /*
         * [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepository.GetUserByEmailAsync(model.UserEmail);

            if (user == null)
            {
                return Unauthorized(new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid email or password."
                });
            }

            if (user.AccountStatus != "Active")
            {
                return Unauthorized(new AuthResponseDTO
                {
                    Success = false,
                    Message = "Your account is inactive."
                });
            }

            bool validPassword = BCrypt.Net.BCrypt.Verify(
                model.Password,
                user.UserHashedPass);

            if (!validPassword)
            {
                return Unauthorized(new AuthResponseDTO
                {
                    Success = false,
                    Message = "Invalid email or password."
                });
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new AuthResponseDTO
            {
                Success = true,
                Message = "Login successful.",
                Token = token,
                User = new UserDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    UserEmail = user.UserEmail,
                    UserType = user.UserType,
                    AccountStatus = user.AccountStatus
                }
            });
        }
        */

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            var agent = Request.Headers.UserAgent.ToString();

            var response =  await _authService.LoginAsync(dto, ip, agent);

            if (response == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid email or password."
                });
            }

            return Ok(new ApiResponse<LoginResponseDTO>
            {
                Success = true,
                Message = "Login successful.",
                Data = response
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDTO dto)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(dto, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString());

                if (response == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid refresh token."
                    });
                }

                return Ok(new ApiResponse<LoginResponseDTO>
                {
                    Success = true,
                    Message = "Token refreshed successfully.",
                    Data = response
                });
            }
            catch (SecurityException)
            {
                return Unauthorized(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Your session has expired or was revoked. Please login again."
                    });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequestDTO dto)
        {
            var ipAddress =
                HttpContext.Connection
                    .RemoteIpAddress?
                    .ToString();

            bool result =
                await _authService.LogoutAsync(
                    dto,
                    ipAddress);

            if (!result)
            {
                return Unauthorized(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid or already revoked refresh token."
                    });
            }

            return Ok(
                new ApiResponse<object>
                {
                    Success = true,
                    Message = "Logout successful.",
                    Data = null
                });
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAllDevices()
        {
            var userIdClaim = User.FindFirst("UserId");

            if (userIdClaim == null)
            {
                return Unauthorized(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid authentication token."
                    });
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid authentication token."
                    });
            }

            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            bool result =
                await _authService.LogoutAllDevicesAsync(userId, ipAddress);

            if (!result)
            {
                return BadRequest(
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message ="Unable to logout from all devices."
                    });
            }

            return Ok(
                new ApiResponse<object>
                {
                    Success = true,
                    Message ="Logged out from all devices successfully.",
                    Data = null
                });
        }
    }
}