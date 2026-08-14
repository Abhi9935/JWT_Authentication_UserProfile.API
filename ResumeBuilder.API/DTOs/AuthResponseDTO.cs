namespace ResumeBuilder.API.DTOs
{
    public class AuthResponseDTO
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? Token { get; set; }

        public UserDTO? User { get; set; }
    }
}