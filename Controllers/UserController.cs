using System;
using Microsoft.AspNetCore.Mvc;
using TodoList.Services;
using TodoList.Models;
using MongoDB.Driver.Core.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
//using TodoList.Handler;


namespace TodoList.Controllers;

[Controller]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly MongoDBService _MongoDBService;
    private readonly JwtSettings jwtSettings;

    public UserController(MongoDBService mongoDBService, IOptions<JwtSettings> options)
    {
        _MongoDBService = mongoDBService;
        this.jwtSettings = options.Value;
    }

    [HttpGet]
    [Authorize]
    public async Task<List<User>> Get()
    {
        return await _MongoDBService.GetAsync();
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        await _MongoDBService.Register(user);
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }


    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] User user)
    {
        var loggedInUser = await _MongoDBService.Login(user.Username, user.Password);
        if (loggedInUser == null)
        {
            return Unauthorized("Invalid username or password");
        }
        //genarate token
        var tokenhandler = new JwtSecurityTokenHandler();
        var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securitykey);

        // Here, ensure that loggedInUser.Id is of the correct type (e.g., string)
        var userIdentity = new ClaimsIdentity(new Claim[]
        {
        new Claim(ClaimTypes.Name, loggedInUser.Username) // Assuming loggedInUser.Id is of type ObjectId
        });
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = userIdentity,
            Expires = DateTime.UtcNow.AddMinutes(20), // Use UtcNow for consistency
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenhandler.CreateToken(tokenDescriptor);
        string finaltoken = tokenhandler.WriteToken(token);

        return Ok(finaltoken);
    }

}