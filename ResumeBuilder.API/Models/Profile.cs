using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeBuilder.API.Models
{
    [Table("Profile")]
    public class Profile
    {
        [Key]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Sex { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FathersName { get; set; } = string.Empty;

        public DateOnly Dob { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Nationality { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Relationship { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Languages { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Interests { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Hobbies { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Image { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}