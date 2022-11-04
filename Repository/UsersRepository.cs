using Auth_API.Data;
using Auth_API.Models;
using Auth_API.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth_API.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration config;

        public UsersRepository(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            this.config = config;
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

        public bool ValidToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var SecretKey = config.GetValue<string>("AppSettings:Token");
                var key = Encoding.ASCII.GetBytes(SecretKey);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "unique_name").Value;

                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public Users GetUserName(string userEmail)
        {
            var user = _db.User.FirstOrDefault(u => u.UserEmail == userEmail);

            return user;
        }

        public string CreateToken(string userId, string userEmail)
        {
            try
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, userEmail)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Token").Value));

                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = credentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                var defTkn = tokenHandler.WriteToken(token);

                return defTkn;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
