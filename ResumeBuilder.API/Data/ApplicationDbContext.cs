using Microsoft.EntityFrameworkCore;
using ResumeBuilder.API.Models;
using ResumeBuilder.API.Models.JWT.Models;

namespace ResumeBuilder.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.UserEmail)
                      .IsUnique();
                entity.Property(e => e.AccountStatus)
                      .HasDefaultValue("Active");

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // For SQL Server, you can also use GETDATE() or SYSDATETIME() instead of CURRENT_TIMESTAMP if you prefer
            });
            modelBuilder.Entity<Profile>()
                    .HasOne(p => p.User)
                    .WithOne(u => u.Profile)
                    .HasForeignKey<Profile>(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

            /*
            modelBuilder.Entity<RefreshToken>()
                       .HasOne(x => x.User)
                       .WithMany(x => x.RefreshTokens)
                       .HasForeignKey(x => x.UserId);
            */
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(x => x.RefreshTokenId);
                entity.Property(x => x.TokenHash)
                      .HasMaxLength(64)
                      .IsRequired();
                entity.Property(x => x.CreatedAt)
                      .HasDefaultValueSql("SYSUTCDATETIME()");
                entity.HasOne(x => x.User)
                      .WithMany(x => x.RefreshTokens)
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(x => x.TokenHash)
                      .IsUnique();
                entity.HasIndex(x => x.UserId);
                entity.HasIndex(x => x.ExpiresAt);
            });
        }
    }
}