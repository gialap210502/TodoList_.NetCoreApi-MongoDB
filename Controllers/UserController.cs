using System;
using Microsoft.AspNetCore.Mvc;
using TodoList.Services;
using TodoList.Models;
using MongoDB.Driver.Core.Authentication;

namespace TodoList.Controllers;

[Controller]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly MongoDBService _MongoDBService;

    public UserController(MongoDBService mongoDBService)
    {
        _MongoDBService = mongoDBService;
    }

    [HttpGet]
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
        return Ok(loggedInUser);
    }

}