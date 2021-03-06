using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entites;
using API.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class AccountController : BaseApiController
  {
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    public AccountController(DataContext context, ITokenService tokenService)
    {
      _tokenService = tokenService;
      _context = context;
    }
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> register(registerDto registerDto)
    {
      if (await UserExists(registerDto.Username)) return BadRequest("username is taken");
      using var hmac = new HMACSHA512();
      var user = new AppUser
      {
        userName = registerDto.Username.ToLower(),
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        passwordSalt = hmac.Key
      };
      _context.Users.Add(user);
      await _context.SaveChangesAsync();
      return new UserDto
      {
        Username = user.userName,
        Token = _tokenService.CreateToken(user)
      };
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(loginDto loginDto)
    {
      var user = await _context.Users
      .SingleOrDefaultAsync(x => x.userName == loginDto.Username);
      if (user == null) return Unauthorized("Invalid username");
      using var hmac = new HMACSHA512(user.passwordSalt);
      var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
      for (int i = 0; i < ComputeHash.Length; i++)
      {
        if (ComputeHash[i] != user.passwordHash[i]) return Unauthorized("Invalid password");
      }
      return new UserDto
      {
        Username = user.userName,
        Token = _tokenService.CreateToken(user)
      };
    }
    private async Task<bool> UserExists(string username)
    {
      return await _context.Users.AnyAsync(x => x.userName == username.ToLower());

    }
  }
}