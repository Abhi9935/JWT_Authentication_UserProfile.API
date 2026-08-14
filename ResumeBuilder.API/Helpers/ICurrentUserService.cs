namespace ResumeBuilder.API.Helpers
{
    public interface ICurrentUserService
    {
        int UserId { get; }

        string Username { get; }

        string Role { get; }

        bool IsAuthenticated { get; }
    }
}