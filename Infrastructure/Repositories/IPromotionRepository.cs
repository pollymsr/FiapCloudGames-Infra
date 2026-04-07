using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Infrastructure.Repositories;

public interface IPromotionRepository
{
    Task AddAsync(Promotion promotion);
    Task UpdateAsync(Promotion promotion);
    Task DeleteAsync(Promotion promotion);
    Task<List<Promotion>> GetAllAsync();
    Task<Promotion?> GetByIdAsync(Guid id);
    Task<Promotion?> GetByCodeAsync(string code);
    Task<bool> SaveChangesAsync();
}
