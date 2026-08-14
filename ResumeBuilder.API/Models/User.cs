using ResumeBuilder.API.Models.JWT.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeBuilder.API.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(60)]
        public string Username { get; set; }

        [Required]
        [StringLength(10)]
        public string UserType { get; set; }

        [Required]
        [StringLength(255)]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(60)]
        public string UserHashedPass { get; set; }

        [StringLength(20)]
        public string? AccountStatus { get; set; } = "Active";

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // Navigation Property
        public virtual Profile? Profile { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
