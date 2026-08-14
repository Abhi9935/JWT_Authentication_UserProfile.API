namespace ResumeBuilder.API.Model.Users
{
    public class Education
    {
        public int ExamId { get; set; }
        public int UserId { get; set; }
        public string Exam { get; set; }
        public string Board { get; set; }
        public string Year { get; set; }
        public string College { get; set; }
        public float Percentage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
