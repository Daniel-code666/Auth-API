using Auth_API.Models;
using Auth_API.Models.Dtos;
using Auth_API.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UsersController(IUsersRepository userRepo, IMapper mapper, IConfiguration config)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost]
        public IActionResult Register(UserAuthDto usuarioAuthDto)
        {
            try
            {
                if (_userRepo.UserExists(usuarioAuthDto.UserEmail.ToLower())) return BadRequest("Usuario ya existe");

                var newUser = new Users
                {
                    UserName = usuarioAuthDto.UserName,
                    UserEmail = usuarioAuthDto.UserEmail
                };

                var createdUser = _userRepo.Register(newUser, usuarioAuthDto.Password);

                return Ok(createdUser);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Login(UserAuthLogin usuarioAuthLoginDto)
        {
            try
            {
                var loggedUser = _userRepo.Login(usuarioAuthLoginDto.UserEmail, usuarioAuthLoginDto.Password);

                if (loggedUser == null) return Unauthorized();

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, loggedUser.UserId.ToString()),
                    new Claim(ClaimTypes.Name, loggedUser.UserEmail.ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = credentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new
                {
                    token = tokenHandler.WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
