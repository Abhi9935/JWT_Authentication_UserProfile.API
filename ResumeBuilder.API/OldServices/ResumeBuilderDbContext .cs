using Microsoft.EntityFrameworkCore;
using ResumeBuilder.API.Model.Users;

namespace ResumeBuilder.API.Services
{
    public class ResumeBuilderDbContext : DbContext
    {
        public ResumeBuilderDbContext(DbContextOptions<ResumeBuilderDbContext> options) : base(options) { }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserDetailsDTO> UserDetailsDTO { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<Achievements> Achievements { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<Employments> Employments { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<Objectives> Objectives { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<Skills> Skills { get; set; }
        public DbSet<SocialIDs> SocialIDs { get; set; }
    }
}
