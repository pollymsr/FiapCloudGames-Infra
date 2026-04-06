using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using FiapCloudGames.Application.DTOs;
using System.Security.Claims;

namespace FiapCloudGames.API.Controllers;

[ApiController]
[Route("api/games")]
[Authorize]
public class GameController : ControllerBase
{
    private readonly AppDbContext _context;

    public GameController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("list")]
    [EndpointName("ListAllGames")]
    public async Task<IActionResult> GetAll()
    {
        var games = await _context.Games.ToListAsync();
        return Ok(games);
    }

    [HttpGet("get/{id}")]
    [EndpointName("GetGameById")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var game = await _context.Games.FindAsync(id);
        if (game == null)
            return NotFound("Game not found");

        return Ok(game);
    }

    [HttpPost("create")]
    [EndpointName("CreateGame")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateGameDto dto)
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            Genre = dto.Genre,
            ReleaseDate = dto.ReleaseDate
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return Ok(game);
    }

    [HttpPut("update/{id}")]
    [EndpointName("UpdateGame")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGameDto dto)
    {
        var game = await _context.Games.FindAsync(id);
        if (game == null)
            return NotFound("Game not found");

        game.Title = dto.Title;
        game.Description = dto.Description;
        game.Price = dto.Price;
        game.Genre = dto.Genre;
        game.ReleaseDate = dto.ReleaseDate;

        await _context.SaveChangesAsync();
        return Ok(game);
    }

    [HttpDelete("delete/{id}")]
    [EndpointName("DeleteGame")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var game = await _context.Games.FindAsync(id);
        if (game == null)
            return NotFound("Game not found");

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
        return Ok("Game removed successfully");
    }

    [HttpPost("buy/{gameId}")]
    [EndpointName("BuyGame")]
    public async Task<IActionResult> BuyGame(Guid gameId)
    {
        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

        if (user == null)
            return Unauthorized("User not found");

        var game = await _context.Games.FindAsync(gameId);
        if (game == null)
            return NotFound("Game not found");

        var alreadyOwned = await _context.UserGames
            .AnyAsync(ug => ug.UserId == user.Id && ug.GameId == gameId);

        if (alreadyOwned)
            return BadRequest("User already owns this game");

        var userGame = new UserGame
        {
            UserId = user.Id,
            GameId = gameId,
            PurchaseDate = DateTime.Now
        };

        _context.UserGames.Add(userGame);
        await _context.SaveChangesAsync();

        // Retornar apenas as informações necessárias (sem loops)
        return Ok(new
        {
            message = $"Game '{game.Title}' purchased successfully!",
            gameId = game.Id,
            gameTitle = game.Title,
            price = game.Price,
            purchaseDate = DateTime.Now
        });
    }


    [HttpGet("my-games")]
    [EndpointName("MyGames")]
    public async Task<IActionResult> GetMyGames()
    {
        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

        if (user == null)
            return Unauthorized();

        var myGames = await _context.UserGames
            .Where(ug => ug.UserId == user.Id)
            .Include(ug => ug.Game)
            .Select(ug => ug.Game)
            .ToListAsync();

        return Ok(myGames);
    }
}