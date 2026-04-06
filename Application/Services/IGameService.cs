using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Application.Services;

public interface IGameService
{
    Task<List<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(Guid id);
    Task<Game> CreateAsync(CreateGameDto dto);
    Task<Game?> UpdateAsync(Guid id, UpdateGameDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<Game?> PurchaseAsync(Guid userId, Guid gameId);
    Task<List<Game>> GetLibraryAsync(Guid userId);
}
