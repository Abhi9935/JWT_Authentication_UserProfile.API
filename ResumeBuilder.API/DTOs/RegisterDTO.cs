using System.ComponentModel.DataAnnotations;

namespace ResumeBuilder.API.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string UserType { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
