using ResumeBuilder.API.DTOs;

namespace ResumeBuilder.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResultDTO<UserDTO>> SearchUsersAsync(UserQueryDTO query);

        Task<IEnumerable<UserDTO>> GetAllUsersAsync();

        Task<UserDTO?> GetUserByIdAsync(int id);

        Task<UserDTO?> UpdateUserAsync(int id, UserDTO DTO);

        Task<bool> DeleteUserAsync(int id);
        Task<UserDTO?> GetCurrentUserAsync();

        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO dto);

        Task<bool> ChangeStatusAsync(int userId, string status);
    }
}