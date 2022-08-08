using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using API.DTOs;
using API.Services;
using API.Interfaces;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    public class AccountController:BaseApiController
    {
        private readonly DataContext _context;

        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService){
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<userDto>> Register(RegisterDTO registerDTO){
            
            if (await this.UserExists(registerDTO.Username)) return BadRequest("Username is taken!");

            using var hmac = new HMACSHA512();
            var user = new AppUser{
                UserName= registerDTO.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new userDto{
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<userDto>> Login(LoginDto loginDTO){
            var user = await _context.Users.SingleOrDefaultAsync(x=>x.UserName==loginDTO.Username.ToLower());
            if (user==null) return Unauthorized("Invalid User name");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDTO.Password));
            for (int i=0; i<computedHash.Length; i++){
                if (computedHash[i]!=user.PasswordHash[i]) return Unauthorized("Invalid password");
            }
            return new userDto{
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };

        }

        private async Task<bool> UserExists(string username){
            return await _context.Users.AnyAsync(x=>x.UserName==username.ToLower());
        }
    }
}