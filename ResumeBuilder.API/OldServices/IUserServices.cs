using ResumeBuilder.API.Model.Users;

namespace ResumeBuilder.API.Services
{
    public interface IUserServicesOld
    {
        List<UserDetailsDTO> GetUsersDetails(bool? isActive);

        Users AddUsers(Users obj);

        Users? UpdateUsers(int id, Users obj);

        bool DeActivateUsersByID(int id);
        bool DeleteUsersByID(int id);
    }
}
