using Auth_API.Models;

namespace Auth_API.Repository.IRepository
{
    public interface IUsersRepository
    {
        ICollection<Users> GetUsers();
        Users GetUser(int userId);
        Users GetUserName(string userEmail);
        bool UserExists(string userEmail);
        Users Register(Users user, string password);
        Users Login(string userEmail, string password);
        bool ValidToken(string token);
        string CreateToken(string userId, string userEmail);
        bool Save();
    }
}
