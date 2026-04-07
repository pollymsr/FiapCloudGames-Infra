using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Services;
using FiapCloudGames.Infrastructure.Repositories;

namespace FiapCloudGames.Application.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGameDomainService _gameDomainService;
    private readonly IPromotionService _promotionService;

    public GameService(
        IGameRepository gameRepository,
        IUserRepository userRepository,
        IGameDomainService gameDomainService,
        IPromotionService promotionService)
    {
        _gameRepository = gameRepository;
        _userRepository = userRepository;
        _gameDomainService = gameDomainService;
        _promotionService = promotionService;
    }

    public async Task<Game> CreateAsync(CreateGameDto dto)
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            Price = dto.Price,
            Genre = dto.Genre.Trim(),
            ReleaseDate = dto.ReleaseDate
        };

        _gameDomainService.ValidateGame(game);
        await _gameRepository.AddAsync(game);
        await _gameRepository.SaveChangesAsync();
        return game;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var game = await _gameRepository.GetByIdAsync(id);
        if (game == null)
            return false;

        await _gameRepository.DeleteAsync(game);
        return await _gameRepository.SaveChangesAsync();
    }

    public async Task<List<Game>> GetAllAsync()
    {
        return await _gameRepository.GetAllAsync();
    }

    public async Task<Game?> GetByIdAsync(Guid id)
    {
        return await _gameRepository.GetByIdAsync(id);
    }

    public async Task<List<Game>> GetLibraryAsync(Guid userId)
    {
        return await _gameRepository.GetGamesForUserAsync(userId);
    }

    public async Task<Game?> PurchaseAsync(Guid userId, Guid gameId, string? promotionCode = null)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return null;

        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game == null)
            return null;

        var alreadyOwned = await _gameRepository.UserOwnsGameAsync(userId, gameId);
        if (alreadyOwned)
            throw new InvalidOperationException("Usuário já possui este jogo.");

        var userGame = new UserGame
        {
            UserId = userId,
            GameId = gameId,
            PurchaseDate = DateTime.UtcNow
        };

        // Aplicar promoção se código fornecido
        if (!string.IsNullOrWhiteSpace(promotionCode))
        {
            var promotion = await _promotionService.GetByCodeAsync(promotionCode);
            if (promotion != null)
            {
                if (!promotion.IsActive)
                    throw new InvalidOperationException("Esta promoção não está ativa.");

                var now = DateTime.UtcNow;
                if (now < promotion.StartDate || now > promotion.EndDate)
                    throw new InvalidOperationException("Esta promoção não está no período válido.");

                if (promotion.CurrentUses >= promotion.MaxUses)
                    throw new InvalidOperationException("Esta promoção atingiu o limite de usos.");

                // Atualizar uso da promoção
                promotion.CurrentUses++;
                await _promotionService.UpdateAsync(promotion.Id, new UpdatePromotionDto
                {
                    Description = promotion.Description,
                    DiscountPercentage = promotion.DiscountPercentage,
                    StartDate = promotion.StartDate,
                    EndDate = promotion.EndDate,
                    IsActive = promotion.IsActive,
                    MaxUses = promotion.MaxUses
                });
            }
            else
            {
                throw new InvalidOperationException("Código de promoção inválido.");
            }
        }

        await _gameRepository.AddUserGameAsync(userGame);
        await _gameRepository.SaveChangesAsync();
        return game;
    }

    public async Task<Game?> UpdateAsync(Guid id, UpdateGameDto dto)
    {
        var game = await _gameRepository.GetByIdAsync(id);
        if (game == null)
            return null;

        game.Title = dto.Title.Trim();
        game.Description = dto.Description.Trim();
        game.Price = dto.Price;
        game.Genre = dto.Genre.Trim();
        game.ReleaseDate = dto.ReleaseDate;

        _gameDomainService.ValidateGame(game);
        await _gameRepository.UpdateAsync(game);
        await _gameRepository.SaveChangesAsync();
        return game;
    }
}
