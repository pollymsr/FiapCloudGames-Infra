// Controllers/GameControllerTddTests.cs
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.API.Controllers;

namespace FiapCloudGames.Tests.Controllers;

public class GameControllerTddTests
{
    // TDD: Primeiro escrevemos o teste, depois implementamos
    [Fact]
    public async Task BuyGame_UserDoesNotOwnGame_ShouldAddToLibrary()
    {
        // ARRANGE - Preparar os dados
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        // Adicionar usuário e jogo
        var user = new User { Id = userId, Email = "test@email.com", Name = "Test", PasswordHash = "hash", Role = "User" };
        var game = new Game { Id = gameId, Title = "Test Game", Price = 100, Description = "Test", Genre = "Action", ReleaseDate = DateTime.Now };
        context.Users.Add(user);
        context.Games.Add(game);
        await context.SaveChangesAsync();

        // Mock do usuário logado
        var controller = new GameController(context);
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test@email.com") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // ACT - Executar a ação
        var result = await controller.BuyGame(gameId);

        // ASSERT - Verificar o resultado
        var okResult = Assert.IsType<OkObjectResult>(result);

        // Verificar se o jogo foi adicionado ao UserGames
        var userGame = await context.UserGames.FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GameId == gameId);
        Assert.NotNull(userGame);
    }

    [Fact]
    public async Task BuyGame_UserAlreadyOwnsGame_ShouldReturnBadRequest()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();

        var user = new User { Id = userId, Email = "test@email.com", Name = "Test", PasswordHash = "hash", Role = "User" };
        var game = new Game { Id = gameId, Title = "Test Game", Price = 100, Description = "Test", Genre = "Action", ReleaseDate = DateTime.Now };
        context.Users.Add(user);
        context.Games.Add(game);

        // Usuário já possui o jogo
        context.UserGames.Add(new UserGame { UserId = userId, GameId = gameId, PurchaseDate = DateTime.Now });
        await context.SaveChangesAsync();

        var controller = new GameController(context);
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, "test@email.com") };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // ACT
        var result = await controller.BuyGame(gameId);

        // ASSERT
        Assert.IsType<BadRequestObjectResult>(result);
    }
}