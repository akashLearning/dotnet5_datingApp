using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : APIBaseController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username))
            {
                return BadRequest("Username already exists. Please try different one.");
            }
            using var hmac = new HMACSHA256();
            var appUser = new AppUser
            {
                UserName = registerDto.Username?.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
            };
            _context.AppUsers.Add(appUser);
            await _context.SaveChangesAsync();
            var userDto = new UserDto()
            {
                Username = appUser.UserName,
                JWTToken = _tokenService.CreateJWTToken(appUser)
            };
            return userDto;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.AppUsers.SingleOrDefaultAsync(u => u.UserName == loginDto.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }
            using var hmac = new HMACSHA256(user.PasswordSalt);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < hash.Length; i++)
            {
                if (hash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid password");
                }
            }
            var userDto = new UserDto()
            {
                Username = user.UserName,
                JWTToken = _tokenService.CreateJWTToken(user)
            };
            return userDto;
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.AppUsers.AnyAsync(user => user.UserName == username);
        }
    }
}