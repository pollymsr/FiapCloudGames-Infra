using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Application.Services;
using System.Security.Claims;

namespace FiapCloudGames.API.Controllers;

[ApiController]
[Route("api/games")]
[Authorize]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var games = await _gameService.GetAllAsync();
        return Ok(games);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var game = await _gameService.GetByIdAsync(id);
        if (game == null)
            return NotFound("Jogo não encontrado");

        return Ok(game);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateGameDto dto)
    {
        var game = await _gameService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGameDto dto)
    {
        var game = await _gameService.UpdateAsync(id, dto);
        if (game == null)
            return NotFound("Jogo não encontrado");

        return Ok(game);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!await _gameService.DeleteAsync(id))
            return NotFound("Jogo não encontrado");

        return NoContent();
    }

    [HttpPost("{id}/purchase")]
    public async Task<IActionResult> Purchase(Guid id)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized("Usuário inválido");

        try
        {
            var game = await _gameService.PurchaseAsync(userId, id);
            if (game == null)
                return NotFound("Jogo não encontrado");

            return Ok(new
            {
                message = $"Jogo '{game.Title}' comprado com sucesso!",
                game.Id,
                game.Title,
                game.Price,
                purchaseDate = DateTime.UtcNow
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("library")]
    public async Task<IActionResult> GetLibrary()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized("Usuário inválido");

        var games = await _gameService.GetLibraryAsync(userId);
        return Ok(games);
    }
}
