using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Infrastructure.Repositories;

public class PromotionRepository : IPromotionRepository
{
    private readonly AppDbContext _context;

    public PromotionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Promotion promotion)
    {
        await _context.Promotions.AddAsync(promotion);
    }

    public async Task UpdateAsync(Promotion promotion)
    {
        _context.Promotions.Update(promotion);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Promotion promotion)
    {
        _context.Promotions.Remove(promotion);
        await Task.CompletedTask;
    }

    public async Task<List<Promotion>> GetAllAsync()
    {
        return await _context.Promotions
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Promotion?> GetByIdAsync(Guid id)
    {
        return await _context.Promotions.FindAsync(id);
    }

    public async Task<Promotion?> GetByCodeAsync(string code)
    {
        return await _context.Promotions
            .FirstOrDefaultAsync(p => p.Code.ToLower() == code.ToLower());
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
