using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repository, IConfiguration config)
        {
            _config = config;
            _repository = repository;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegister user)
        {
            user.UserName = user.UserName.ToLowerInvariant();

            if (await _repository.UserExists(user.UserName))
            {
                return BadRequest("Username already exists!");
            }

            var userToCreate = new User
            {
                Username = user.UserName
            };

            var createdUser = await _repository.Register(userToCreate, user.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLogin user)
        {
            var userModel = await _repository.Login(user.UserName.ToLowerInvariant(), user.Password);

            if (userModel == null)
            {
                return Unauthorized();
            }

            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, userModel.Id.ToString()),
                new Claim(ClaimTypes.Name, userModel.Username)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandler.WriteToken(token) });
        }
    }
}