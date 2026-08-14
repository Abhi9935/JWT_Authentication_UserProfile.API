using ResumeBuilder.API.DTOs;
using ResumeBuilder.API.Helpers;
using ResumeBuilder.API.Repositories.Interfaces;
using ResumeBuilder.API.Services.Interfaces;

namespace ResumeBuilder.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public UserService(IUserRepository repository, ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }
        public async Task<UserDTO?> GetCurrentUserAsync()
        {
            var user = await _repository.GetByIdAsync(_currentUser.UserId);

            if (user == null)
                return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                UserEmail = user.UserEmail,
                UserType = user.UserType,
                AccountStatus = user.AccountStatus
            };
        }
        public async Task<PagedResultDTO<UserDTO>> SearchUsersAsync(UserQueryDTO query)
        {
            var users = await _repository.SearchUsersAsync(
                query.Search,
                query.PageNumber,
                query.PageSize);

            var totalCount =
                await _repository.GetTotalCountAsync(query.Search);

            var items = users.Select(u => new UserDTO
            {
                UserId = u.UserId,
                Username = u.Username,
                UserEmail = u.UserEmail,
                UserType = u.UserType,
                AccountStatus = u.AccountStatus
            });

            return new PagedResultDTO<UserDTO>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize),
                Items = items
            };
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();

            return users.Select(u => new UserDTO
            {
                UserId = u.UserId,
                Username = u.Username,
                UserEmail = u.UserEmail,
                UserType = u.UserType,
                AccountStatus = u.AccountStatus
            });
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                return null;

            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                UserEmail = user.UserEmail,
                UserType = user.UserType,
                AccountStatus = user.AccountStatus
            };
        }

        public async Task<UserDTO?> UpdateUserAsync(int id, UserDTO DTO)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                return null;

            user.Username = DTO.Username;
            user.UserEmail = DTO.UserEmail;
            user.UserType = DTO.UserType;
            user.AccountStatus = DTO.AccountStatus;
            user.UpdatedAt = DateTime.UtcNow;

            _repository.Update(user);

            await _repository.SaveAsync();

            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                UserEmail = user.UserEmail,
                UserType = user.UserType,
                AccountStatus = user.AccountStatus
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                return false;

            _repository.Delete(user);

            await _repository.SaveAsync();

            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            var user = await _repository.GetByIdAsync(userId);

            if (user == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(
                dto.CurrentPassword,
                user.UserHashedPass))
            {
                return false;
            }

            user.UserHashedPass =
                BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            user.UpdatedAt = DateTime.UtcNow;

            _repository.Update(user);

            await _repository.SaveAsync();

            return true;
        }
        public async Task<bool> ChangeStatusAsync(int userId, string status)
        {
            var user = await _repository.GetByIdAsync(userId);

            if (user == null)
                return false;

            user.AccountStatus = status;

            user.UpdatedAt = DateTime.UtcNow;

            _repository.Update(user);

            await _repository.SaveAsync();

            return true;
        }
    }
}