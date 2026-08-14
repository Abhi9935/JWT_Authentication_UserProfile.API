using System.ComponentModel.DataAnnotations;

namespace ResumeBuilder.API.DTOs
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
