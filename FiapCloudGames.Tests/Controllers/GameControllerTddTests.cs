using Xunit;
using Microsoft.EntityFrameworkCore;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using FiapCloudGames.Infrastructure.Repositories;
using FiapCloudGames.Domain.Services;
using FiapCloudGames.Application.Services;
using FiapCloudGames.API.Controllers;
using FiapCloudGames.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FiapCloudGames.Tests.Controllers;

public class GameControllerTddTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        return context;
    }

    private GameController GetGameController(AppDbContext context)
    {
        var gameService = new GameService(new GameRepository(context), new UserRepository(context), new GameDomainService());
        return new GameController(gameService);
    }

    [Fact]
    public async Task BuyGame_UserDoesNotOwnGame_ShouldAddToLibrary()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        var context = GetDbContext();

        var user = new User { Id = userId, Email = "test@email.com", Name = "Test", PasswordHash = "hash", Role = "User" };
        var game = new Game { Id = gameId, Title = "Test Game", Price = 100, Description = "Test", Genre = "Action", ReleaseDate = DateTime.UtcNow };
        context.Users.Add(user);
        context.Games.Add(game);
        await context.SaveChangesAsync();

        var controller = GetGameController(context);
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = await controller.Purchase(gameId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var userGame = await context.UserGames.FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GameId == gameId);
        Assert.NotNull(userGame);
    }

    [Fact]
    public async Task BuyGame_UserAlreadyOwnsGame_ShouldReturnBadRequest()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        var context = GetDbContext();

        var user = new User { Id = userId, Email = "test@email.com", Name = "Test", PasswordHash = "hash", Role = "User" };
        var game = new Game { Id = gameId, Title = "Test Game", Price = 100, Description = "Test", Genre = "Action", ReleaseDate = DateTime.UtcNow };
        context.Users.Add(user);
        context.Games.Add(game);
        context.UserGames.Add(new UserGame { UserId = userId, GameId = gameId, PurchaseDate = DateTime.UtcNow });
        await context.SaveChangesAsync();

        var controller = GetGameController(context);
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        var result = await controller.Purchase(gameId);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
