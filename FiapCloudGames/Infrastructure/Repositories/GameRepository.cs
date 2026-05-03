using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Infrastructure.Repositories;

public class GameRepository : IGameRepository
{
    private readonly AppDbContext _context;

    public GameRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Game game)
    {
        await _context.Games.AddAsync(game);
    }

    public async Task AddUserGameAsync(UserGame userGame)
    {
        await _context.UserGames.AddAsync(userGame);
    }

    public async Task DeleteAsync(Game game)
    {
        _context.Games.Remove(game);
        await Task.CompletedTask;
    }

    public async Task<List<Game>> GetAllAsync()
    {
        return await _context.Games
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Game?> GetByIdAsync(Guid id)
    {
        return await _context.Games.FindAsync(id);
    }

    public async Task<List<Game>> GetGamesForUserAsync(Guid userId)
    {
        return await _context.UserGames
            .Where(ug => ug.UserId == userId)
            .Include(ug => ug.Game)
            .Select(ug => ug.Game)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        return await _context.Games.AnyAsync(g => g.Id == id);
    }

    public async Task<bool> UserOwnsGameAsync(Guid userId, Guid gameId)
    {
        return await _context.UserGames.AnyAsync(ug => ug.UserId == userId && ug.GameId == gameId);
    }

    public async Task UpdateAsync(Game game)
    {
        _context.Games.Update(game);
        await Task.CompletedTask;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
