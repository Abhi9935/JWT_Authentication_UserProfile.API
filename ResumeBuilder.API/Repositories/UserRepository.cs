using Microsoft.EntityFrameworkCore;
using ResumeBuilder.API.Data;
using ResumeBuilder.API.Models;
using ResumeBuilder.API.Repositories.Interfaces;

namespace ResumeBuilder.API.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserEmail == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AsNoTracking()
                .AnyAsync(x => x.UserEmail == email);
        }
        public async Task<IEnumerable<User>> SearchUsersAsync(string? search, int pageNumber, int pageSize)
        {
            IQueryable<User> query = _context.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                //query = query.Where(x =>
                //    x.Username.Contains(search) ||
                //    x.UserEmail.Contains(search));

                query = query.Where(x =>
                        EF.Functions.Like(x.Username, $"%{search}%") ||
                        EF.Functions.Like(x.UserEmail, $"%{search}%"));
            }

            return await query
                .OrderBy(x => x.UserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? search)
        {
            IQueryable<User> query = _context.Users;

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x =>
                    x.Username.Contains(search) ||
                    x.UserEmail.Contains(search));
            }

            return await query.CountAsync();
        }
    }
}