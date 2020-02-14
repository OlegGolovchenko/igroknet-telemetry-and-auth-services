using org.igrok_net.infrastructure.domain.Models;
using System.Collections.Generic;

namespace org.igrok_net.infrastructure.domain.Interfaces
{
    public interface IUserService
    {
        long GetOrCreateUser(string email);
        User GetUser(string email);
        bool UserExists(string email);
        bool UserExists(long userId);
        void RemoveUser(long userId);
        void AssignLicence(long userId, long licenceId);
        void ResignLicence(long userId);
        List<UserListModel> ListUsers();
    }
}
