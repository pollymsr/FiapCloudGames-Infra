using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Application.Services;

public interface IPromotionService
{
    Task<Promotion> CreateAsync(CreatePromotionDto dto);
    Task<Promotion?> UpdateAsync(Guid id, UpdatePromotionDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<List<Promotion>> GetAllAsync();
    Task<Promotion?> GetByIdAsync(Guid id);
    Task<Promotion?> GetByCodeAsync(string code);
    Task<decimal> CalculateDiscountAsync(Guid promotionId, decimal originalPrice);
}
