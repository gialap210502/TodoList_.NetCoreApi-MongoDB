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

namespace TodoList.Controllers;

[Controller]
[Route("api/[controller]")]
public class TodoItemController : Controller
{
    private readonly MongoDBService _MongoDBService;
    private readonly JwtSettings jwtSettings;

    public TodoItemController(MongoDBService mongoDBService, IOptions<JwtSettings> options)
    {
        _MongoDBService = mongoDBService;
        this.jwtSettings = options.Value;
    }

    [HttpPost("GetTodoList")]
    public async Task<List<TodoItem>> Get([FromBody] string userId)
    {
        return await _MongoDBService.GetTodoItemsAsync(userId);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TodoItem todoItem)
    {
        await _MongoDBService.Create(todoItem);
        return Ok(todoItem);
    }
}