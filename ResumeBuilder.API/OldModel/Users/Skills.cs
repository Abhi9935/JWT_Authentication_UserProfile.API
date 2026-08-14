namespace ResumeBuilder.API.Model.Users
{
    public class Skills
    {
        public int SkillsId { get; set; }
        public int UserId { get; set; }
        public string SkillsName { get; set; }
        public string SpecializationType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
    }
}
