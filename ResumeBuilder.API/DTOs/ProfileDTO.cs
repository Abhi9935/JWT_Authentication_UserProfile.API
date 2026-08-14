using System.ComponentModel.DataAnnotations;

namespace ResumeBuilder.API.DTOs
{
    public class ProfileDTO
    {
        public int UserId { get; set; }      
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string FathersName { get; set; }
        public DateOnly Dob { get; set; }
        public string Email { get; set; }        
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Nationality { get; set; }
        public string Relationship { get; set; }
        public string Languages { get; set; }
        public string Interests { get; set; }
        public string Hobbies { get; set; }
        public string Image { get; set; }
        public string Status { get; set; }
    }
}
