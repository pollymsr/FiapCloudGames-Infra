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

    [HttpGet("list")]
    [AllowAnonymous]
    public async Task<IActionResult> ListAllGames()
    {
        var games = await _gameService.GetAllAsync();
        return Ok(games);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetGameById(Guid id)
    {
        var game = await _gameService.GetByIdAsync(id);
        if (game == null)
            return NotFound("Jogo não encontrado");

        return Ok(game);
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameDto dto)
    {
        var game = await _gameService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, game);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateGameById(Guid id, [FromBody] UpdateGameDto dto)
    {
        var game = await _gameService.UpdateAsync(id, dto);
        if (game == null)
            return NotFound("Jogo não encontrado");

        return Ok(game);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteGameById(Guid id)
    {
        if (!await _gameService.DeleteAsync(id))
            return NotFound("Jogo não encontrado");

        return NoContent();
    }

    [HttpPost("{id}/purchase")]
    public async Task<IActionResult> BuyGame(Guid id, [FromQuery] string? promotionCode = null)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized("Usuário inválido");

        try
        {
            var game = await _gameService.PurchaseAsync(userId, id, promotionCode);
            if (game == null)
                return NotFound("Jogo não encontrado");

            return Ok(new
            {
                message = $"Jogo '{game.Title}' comprado com sucesso!",
                game.Id,
                game.Title,
                game.Price,
                purchaseDate = DateTime.UtcNow,
                promotionApplied = !string.IsNullOrWhiteSpace(promotionCode)
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("library")]
    public async Task<IActionResult> GetMyLibrary()
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized("Usuário inválido");

        var games = await _gameService.GetLibraryAsync(userId);
        return Ok(games);
    }
}
