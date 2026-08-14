using System.ComponentModel.DataAnnotations;

namespace ResumeBuilder.API.Model.Users
{
    public class Employments
    {
        public int Eid { get; set; }
        public int UserId { get; set; }
        public string FromYear { get; set; }
        public string ToYear { get; set; }
        public string Company { get; set; }
        public string Designation { get; set; }
        public string Detail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
    }
}
