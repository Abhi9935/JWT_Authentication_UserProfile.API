namespace ResumeBuilder.API.Model.Users
{
    public class Objectives
    {
        public int UserId { get; set; }
        public string ResumeType { get; set; }
        public string Obj { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
