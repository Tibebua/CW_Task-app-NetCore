using CW_Tasks_app_NetCore.Data;
using CW_Tasks_app_NetCore.Dto;
using CW_Tasks_app_NetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CW_Tasks_app_NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly TaskDbContext _context;
        private readonly IConfiguration _config;

        public AccountController(TaskDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost(nameof(Register))]
        public async Task<IActionResult> Register(UserRegisterDTO userRegisterDTO) 
        {  // basically, register is simply saving the new user to database (ofcourse, with password hashed)

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(await UserNameExists(userRegisterDTO.Username.ToLower()))
            {
                return BadRequest("Username already exists");
            }
            
            var newUser = new User();

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(userRegisterDTO.Password, out passwordHash, out passwordSalt);

            newUser.Username = userRegisterDTO.Username;
            newUser.PasswordHash = passwordHash;
            newUser.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok(newUser);
        }

        private async Task<bool> UserNameExists(string username)
        {
            if(await _context.Users.AnyAsync(u => u.Username == username))
            {
                return true;
            }
            return false;
        }

        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(UserLoginDto userLoginDTO)
        {  // basically, login is simply getting the user from DB, calculating the entered pwd's hash 
           // and comparing it with the one on DB

            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == userLoginDTO.Username);

            if(userInDb == null)
            {
                return Unauthorized();
            }

            if(!VerifyPassword(userLoginDTO.Password, userInDb.PasswordHash, userInDb.PasswordSalt))
            {
                return Unauthorized();
            }

            // If it got here, it means user is legit... let's give him a token. 
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userInDb.Id.ToString()),
                new Claim(ClaimTypes.Name, userInDb.Username.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JwtOptions:SecretKey").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { 
                token = tokenHandler.WriteToken(token)
            });
            
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for(int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
