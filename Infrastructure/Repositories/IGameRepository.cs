using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Infrastructure.Repositories;

public interface IGameRepository
{
    Task<List<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(Guid id);
    Task AddAsync(Game game);
    Task UpdateAsync(Game game);
    Task DeleteAsync(Game game);
    Task<bool> ExistsByIdAsync(Guid id);
    Task<bool> UserOwnsGameAsync(Guid userId, Guid gameId);
    Task AddUserGameAsync(UserGame userGame);
    Task<List<Game>> GetGamesForUserAsync(Guid userId);
    Task<bool> SaveChangesAsync();
}
