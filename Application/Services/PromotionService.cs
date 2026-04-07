using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Services;
using FiapCloudGames.Infrastructure.Repositories;

namespace FiapCloudGames.Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IPromotionRepository _promotionRepository;
    private readonly IPromotionDomainService _promotionDomainService;

    public PromotionService(
        IPromotionRepository promotionRepository,
        IPromotionDomainService promotionDomainService)
    {
        _promotionRepository = promotionRepository;
        _promotionDomainService = promotionDomainService;
    }

    public async Task<Promotion> CreateAsync(CreatePromotionDto dto)
    {
        var existingPromotion = await _promotionRepository.GetByCodeAsync(dto.Code);
        if (existingPromotion != null)
            throw new InvalidOperationException("Já existe uma promoção com este código.");

        var promotion = new Promotion
        {
            Id = Guid.NewGuid(),
            Code = dto.Code.ToUpper().Trim(),
            Description = dto.Description.Trim(),
            DiscountPercentage = dto.DiscountPercentage,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = true,
            MaxUses = dto.MaxUses,
            CurrentUses = 0,
            CreatedAt = DateTime.UtcNow
        };

        _promotionDomainService.ValidatePromotion(promotion);
        await _promotionRepository.AddAsync(promotion);
        await _promotionRepository.SaveChangesAsync();
        return promotion;
    }

    public async Task<Promotion?> UpdateAsync(Guid id, UpdatePromotionDto dto)
    {
        var promotion = await _promotionRepository.GetByIdAsync(id);
        if (promotion == null)
            return null;

        promotion.Description = dto.Description.Trim();
        promotion.DiscountPercentage = dto.DiscountPercentage;
        promotion.StartDate = dto.StartDate;
        promotion.EndDate = dto.EndDate;
        promotion.IsActive = dto.IsActive;
        promotion.MaxUses = dto.MaxUses;

        _promotionDomainService.ValidatePromotion(promotion);
        await _promotionRepository.UpdateAsync(promotion);
        await _promotionRepository.SaveChangesAsync();
        return promotion;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var promotion = await _promotionRepository.GetByIdAsync(id);
        if (promotion == null)
            return false;

        await _promotionRepository.DeleteAsync(promotion);
        return await _promotionRepository.SaveChangesAsync();
    }

    public async Task<List<Promotion>> GetAllAsync()
    {
        return await _promotionRepository.GetAllAsync();
    }

    public async Task<Promotion?> GetByIdAsync(Guid id)
    {
        return await _promotionRepository.GetByIdAsync(id);
    }

    public async Task<Promotion?> GetByCodeAsync(string code)
    {
        return await _promotionRepository.GetByCodeAsync(code);
    }

    public async Task<decimal> CalculateDiscountAsync(Guid promotionId, decimal originalPrice)
    {
        var promotion = await _promotionRepository.GetByIdAsync(promotionId);
        if (promotion == null)
            return originalPrice;

        if (!_promotionDomainService.IsPromotionValid(promotion))
            throw new InvalidOperationException("Esta promoção não está mais válida.");

        var discountAmount = originalPrice * (promotion.DiscountPercentage / 100);
        return originalPrice - discountAmount;
    }
}
