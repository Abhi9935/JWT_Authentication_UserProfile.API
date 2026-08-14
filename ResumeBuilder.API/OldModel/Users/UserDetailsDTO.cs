using System.Runtime.CompilerServices;

namespace ResumeBuilder.API.Model.Users
{
    public class UserDetailsDTO
    {
        public Users Usersdetails {  get; set; }
        public UserProfile UsersProfileDetails {  get; set; }
        public Achievements achievements {  get; set; }
        public Education education {  get; set; }
        public Employments employments {  get; set; }
        public Feedback feedback {  get; set; }
        public Objectives objectives {  get; set; }
        public Projects Projects {  get; set; }
        public Skills Skills {  get; set; }
        public SocialIDs SocialAccount {  get; set; }
    }
}
