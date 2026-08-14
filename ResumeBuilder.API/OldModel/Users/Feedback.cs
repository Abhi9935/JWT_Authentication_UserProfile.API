namespace ResumeBuilder.API.Model.Users
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int UserId { get; set; }
        public string FeedbackMsg { get; set; }
        public int Rating { get; set; }
        public int AdminId { get; set; }
        public string Response { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
               
    }
}
