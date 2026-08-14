using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.API.DTOs;
using ResumeBuilder.API.Helpers;
using ResumeBuilder.API.Models;
using ResumeBuilder.API.Repositories.Interfaces;
using ResumeBuilder.API.Services.Interfaces;

namespace ResumeBuilder.API.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller //ControllerBase   TBC the Diff btw ControllerBase vs Controller
    {
        private readonly IUserService _service;
        private readonly ICurrentUserService _currentUser;

        public UsersController(IUserService service, ICurrentUserService currentUser)
        {
            _service = service;
            _currentUser = currentUser;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser() // GetCurrentUser
        {
            var user = await _service.GetCurrentUserAsync();

            if (user == null)
                return NotFound();

            return Ok(new ApiResponse<UserDTO>
            {
                Success = true,
                Message = "Profile loaded.",
                Data = user
            });
        }

        //[HttpGet("me")]
        //public async Task<IActionResult> GetCurrentUser()
        //{
        //    var idClaim = User.FindFirst("UserId");

        //    if (idClaim == null)
        //        return Unauthorized();

        //    int userId = int.Parse(idClaim.Value);

        //    var user = await _service.GetUserByIdAsync(userId);

        //    if (user == null)
        //        return NotFound();

        //    /*return Ok(new
        //    {
        //        user.UserId,
        //        user.Username,
        //        user.UserEmail,
        //        user.UserType,
        //        user.AccountStatus
        //    });
        //    */
        //    return Ok(new ApiResponse<UserDTO>
        //    {
        //        Success = true,
        //        Message = "User retrieved successfully.",
        //        Data = user
        //    });
        //}

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("JWT Authentication Successful");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] UserQueryDTO query)
        {
            var result = await _service.SearchUsersAsync(query);

            return Ok(new ApiResponse<PagedResultDTO<UserDTO>>
            {
                Success = true,
                Message = "Users retrieved successfully.",
                Data = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _service.GetAllUsersAsync();

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _service.GetUserByIdAsync(id);

            if (user == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found."
                });

            //return Ok(user);
            return Ok(new ApiResponse<UserDTO>
            {
                Success = true,
                Message = "User retrieved successfully.",
                Data = user
            });
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDTO dto)
        {
            if (_currentUser.Role != "Admin" && _currentUser.UserId != id)
            {
                throw new UnauthorizedAccessException(
                    "You can only update your own profile.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _service.UpdateUserAsync(id, dto);

            if (user == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found."
                });

            return Ok(new ApiResponse<UserDTO>
            {
                Success = true,
                Message = "User updated successfully.",
                Data = user
            });
        }

        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_currentUser.Role != "Admin")
            {
                throw new UnauthorizedAccessException(
                    "Only admin can delete profile.");
            }

            var deleted = await _service.DeleteUserAsync(id);

            if (!deleted)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found."
                });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User deleted successfully."
            });
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordDTO dto)
        {
            if (_currentUser.UserId != id)
            {
                throw new UnauthorizedAccessException(
                    "You can only update your own password.");
            }

            var result = await _service.ChangePasswordAsync(_currentUser.UserId, dto);

            if (!result)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Current password is incorrect."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Password changed successfully."
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            var result = await _service.ChangeStatusAsync(id, status);

            if (!result)
                return NotFound();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Status updated."
            });
        }

    }
}     
   