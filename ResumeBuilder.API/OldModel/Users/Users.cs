using System.Runtime.Intrinsics.X86;

namespace ResumeBuilder.API.Model.Users
{
    public class Users
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public string UserType { get; set; } 
        public string UserEmail { get; set; }
        public string AccountStatus { get; set; }
        public DateTime CreatedAt { get; set;  }
        public DateTime UpdatedAt { get; set;  }
    }
}
