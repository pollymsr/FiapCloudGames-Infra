using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Services;
using FiapCloudGames.Infrastructure.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace FiapCloudGames.Tests.Services;

public class GameServiceTests
{
    private readonly Mock<IGameRepository> _gameRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IGameDomainService> _gameDomainServiceMock;
    private readonly Mock<IPromotionService> _promotionServiceMock;
    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _gameRepositoryMock = new Mock<IGameRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _gameDomainServiceMock = new Mock<IGameDomainService>();
        _promotionServiceMock = new Mock<IPromotionService>();
        
        _gameService = new GameService(
            _gameRepositoryMock.Object, 
            _userRepositoryMock.Object, 
            _gameDomainServiceMock.Object, 
            _promotionServiceMock.Object);
    }

    [Fact]
    public async Task PurchaseAsync_ShouldSucceed_WhenUserAndGameExist()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var user = new User { Id = userId };
        var game = new Game { Id = gameId, Title = "Epic Game" };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(game);
        _gameRepositoryMock.Setup(r => r.UserOwnsGameAsync(userId, gameId)).ReturnsAsync(false);

        var result = await _gameService.PurchaseAsync(userId, gameId);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Epic Game");
        _gameRepositoryMock.Verify(r => r.AddUserGameAsync(It.IsAny<UserGame>()), Times.Once);
        _gameRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task PurchaseAsync_ShouldThrowException_WhenUserAlreadyOwnsGame()
    {
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();
        var user = new User { Id = userId };
        var game = new Game { Id = gameId };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _gameRepositoryMock.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(game);
        _gameRepositoryMock.Setup(r => r.UserOwnsGameAsync(userId, gameId)).ReturnsAsync(true);

        var act = async () => await _gameService.PurchaseAsync(userId, gameId);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Usuário já possui este jogo.");
    }

    [Fact]
    public async Task CreateAsync_ShouldCallDomainValidation()
    {
        var dto = new CreateGameDto { Title = "New Game", Price = 50, Genre = "Action", Description = "Desc" };

        await _gameService.CreateAsync(dto);

        _gameDomainServiceMock.Verify(s => s.ValidateGame(It.IsAny<Game>()), Times.Once);
        _gameRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Once);
    }
}
