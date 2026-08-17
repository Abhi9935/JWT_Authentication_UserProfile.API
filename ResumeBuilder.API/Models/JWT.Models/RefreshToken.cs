using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeBuilder.API.Models.JWT.Models
{
    [Table("RefreshTokens")]
    public class RefreshToken
    {
        [Key]
        public int RefreshTokenId { get; set; }

        public int UserId { get; set; }
        public Guid TokenFamilyId { get; set; }

        [Required]
        [StringLength(64)]
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        [StringLength(64)]
        public string? ReplacedByTokenHash { get; set; }

        [StringLength(50)]
        public string? CreatedByIp { get; set; }

        [StringLength(50)]
        public string? RevokedByIp { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }
        public virtual User User { get; set; } = null!;

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        [NotMapped]
        public bool IsRevoked => RevokedAt.HasValue;

        [NotMapped]
        public bool IsActive => !IsExpired && !IsRevoked;
    }
}