using System.Runtime.Intrinsics.X86;

namespace ResumeBuilder.API.Model
{
    public class UsersLoginDTO
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public string UserType { get; set; } 
        public string UserEmail { get; set; }
        public string UserHashedPass { get; set; }
        public string AccountStatus { get; set; }
        public DateTime CreatedAt { get; set;  }
        public DateTime UpdatedAt { get; set;  }
    }
}
