using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ResumeBuilder.API.Model.Users
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string Fathersname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Nationality { get; set; }
        public string Relationship { get; set; }
        public string Languages { get; set; }
        public string Interests { get; set; }
        public string Hobbies { get; set; }
        public string ImagePath { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }        
    }
}
