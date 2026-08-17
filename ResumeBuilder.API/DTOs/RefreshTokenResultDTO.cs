using static ResumeBuilder.API.EnumsCodes;

namespace ResumeBuilder.API.DTOs
{
    public class RefreshTokenResultDTO
    {
        public RefreshTokenResult Result { get; set; }

        public LoginResponseDTO? Tokens { get; set; }

        public int RevokedTokenCount { get; set; }
    }
}
