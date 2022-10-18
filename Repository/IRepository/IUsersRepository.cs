using Auth_API.Models;

namespace Auth_API.Repository.IRepository
{
    public interface IUsersRepository
    {
        ICollection<Users> GetUsers();
        Users GetUser(int userId);
        bool UserExists(string userEmail);
        Users Register(Users user, string password);
        Users Login(string userEmail, string password);
        string ValidToken(string token);
        bool Save();
    }
}
