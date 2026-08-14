using ResumeBuilder.API.Model.Users;

namespace ResumeBuilder.API.Services
{
    public interface IResumebuilderServices
    {
        UserDetailsDTO GetUsersAllDetails(int id);

        UserProfile GetUsersProfile(int id);
        UserProfile AddUsersProfile( UserProfile obj);
        UserProfile UpdateUsersProfile( UserProfile obj);

        Achievements GetUsersAchievements(int id);
        Achievements AddUsersAchievements(Achievements obj);
        Achievements UpdateUsersAchievements(Achievements obj);
        Achievements DeleteUsersAchievements(Achievements obj);

        Education GetUsersEducation(int id);
        Education AddUsersEducation( Education obj);
        Education UpdateUsersEducation( Education obj);
        Education DeleteUsersEducation( Education obj);

        Employments GetUsersEmployments(int id);
        Employments AddUsersEmployments( Employments obj);
        Employments UpdateUsersEmployments( Employments obj);
        Employments DeleteUsersEmploymentss( Employments obj);

        //Feedback GetUsersFeedback(int id);
        Feedback AddUsersFeedback( Feedback obj);
        Feedback UpdateUsersFeedback( Feedback obj);
        
        Objectives GetUsersObjectives(int id);
        Objectives AddUsersObjectives( Objectives obj);
        Objectives UpdateUsersObjectives( Objectives obj);
        
        Projects GetUsersProjects(int id);
        Projects AddUsersProjects( Projects obj);
        Projects UpdateUsersProjects( Projects obj);
        Projects DeleteUsersProjects( Projects obj);
        
        Skills GetUsersSkills(int id);
        Skills AddUsersSkills( Skills obj);
        Skills UpdateUsersSkills( Skills obj);
        Skills DeleteUsersSkills( Skills obj);
        
        SocialIDs GetUsersSocialIDs(int id);
        SocialIDs AddUsersSocialIDs( SocialIDs obj);
        SocialIDs UpdateUsersSocialIDs( SocialIDs obj);

        bool DeActivateUsersByID(int id);
        bool DeleteUsersByID(int id);
        object GetUserByID(int id);
    }
}
