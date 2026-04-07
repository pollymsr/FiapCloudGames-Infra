using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Services;

public class PromotionDomainService : IPromotionDomainService
{
    public void ValidatePromotion(Promotion promotion)
    {
        if (string.IsNullOrWhiteSpace(promotion.Code))
            throw new InvalidOperationException("O código da promoção não pode ficar vazio.");

        if (promotion.DiscountPercentage < 0 || promotion.DiscountPercentage > 100)
            throw new InvalidOperationException("O percentual de desconto deve estar entre 0 e 100.");

        if (promotion.EndDate <= promotion.StartDate)
            throw new InvalidOperationException("A data de término deve ser posterior à data de início.");

        if (promotion.MaxUses <= 0)
            throw new InvalidOperationException("O número máximo de usos deve ser maior que zero.");
    }

    public bool IsPromotionValid(Promotion promotion)
    {
        var now = DateTime.UtcNow;
        return promotion.IsActive 
            && now >= promotion.StartDate 
            && now <= promotion.EndDate 
            && !HasReachedMaxUses(promotion);
    }

    public bool HasReachedMaxUses(Promotion promotion)
    {
        return promotion.CurrentUses >= promotion.MaxUses;
    }
}
