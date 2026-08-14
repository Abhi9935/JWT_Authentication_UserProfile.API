using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeBuilder.API.Model.Users
{
    public class Projects
    {
        public int Pid { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Type { get; set; }
        public string Detail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
