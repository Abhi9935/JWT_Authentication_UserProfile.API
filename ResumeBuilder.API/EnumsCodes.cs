namespace ResumeBuilder.API
{
    public class EnumsCodes
    {
        public enum Skills
        {
            Specialization=1,
            Intermediate=2,
            Basics=3,
            Others=4
        }
        public enum AccountStatus
        {
            Active,
            Inactive,
            Suspended
        }
        public enum RefreshTokenResult
        {
            Success,

            InvalidToken,

            Expired,

            ReplayDetected,

            UserNotFound,

            AccountInactive
        }
    }
}
