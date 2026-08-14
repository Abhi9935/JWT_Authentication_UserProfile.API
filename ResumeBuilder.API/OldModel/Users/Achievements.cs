namespace ResumeBuilder.API.Model.Users
{
    public class Achievements
    {
        public int Aid { get; set; }
        public int UserId { get; set; }
        public string Awards { get; set; }
        public string Subtitle { get; set; }
        public string Place { get; set; }
        public string Year { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
                
    }
}
