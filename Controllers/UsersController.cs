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

                var token = _userRepo.CreateToken(loggedUser.UserName, loggedUser.UserEmail);

                var username = _userRepo.GetUserName(loggedUser.UserEmail);

                return Ok(new
                {
                    token,
                    username,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult ValidToken(string token)
        {
            try
            {
                var result = _userRepo.ValidToken(token);
                if (result)
                {
                    return Ok(new {msg = result});
                }
                else
                {
                    return Unauthorized(new {msg = result});
                }
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult GetUserName(UserAuthLogin user)
        {
            try
            {
                var singleUser = _userRepo.GetUserName(user.UserEmail);
                return Ok(new {name = singleUser.UserName});
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                return Ok(_userRepo.GetUsers());
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
