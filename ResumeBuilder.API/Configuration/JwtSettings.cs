namespace ResumeBuilder.API.Configuration
{
    public class JwtSettings123
    {
        public string Key { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public int AccessTokenMinutes { get; set; }

        public int RefreshTokenDays { get; set; }
    }
}
