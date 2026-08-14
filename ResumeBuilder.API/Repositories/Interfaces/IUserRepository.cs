using ResumeBuilder.API.Models;

namespace ResumeBuilder.API.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<bool> EmailExistsAsync(string email);
        Task<IEnumerable<User>> SearchUsersAsync(string? search, int pageNumber, int pageSize);
        Task<int> GetTotalCountAsync(string? search);
    }
}
