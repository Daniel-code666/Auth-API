using Auth_API.Data;
using Auth_API.Models;
using Auth_API.Repository.IRepository;

namespace Auth_API.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _db;

        public UsersRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public ICollection<Users> GetUsers()
        {
            return _db.User.OrderBy(u => u.UserId).ToList();
        }

        public Users GetUser(int userId)
        {
            return _db.User.FirstOrDefault(u => u.UserId == userId);
        }

        public Users Login(string userEmail, string password)
        {
            var user = _db.User.FirstOrDefault(u => u.UserEmail == userEmail);

            if (user == null || !VerificaPasswordHash(password, user.PasswordHash, user.PasswordSalt)) return null;

            return user;
        }

        public Users Register(Users user, string password)
        {
            byte[] passwordHash, passwordSalt;

            CrearPasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _db.User.Add(user);
            Save();

            return user;
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0;
        }

        public bool UserExists(string userEmail)
        {
            return _db.User.Any(u => u.UserEmail == userEmail);
        }

        private bool VerificaPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < hash.Length; i++)
                {
                    if (hash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        private void CrearPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
