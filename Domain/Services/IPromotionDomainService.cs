using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Services;

public interface IPromotionDomainService
{
    void ValidatePromotion(Promotion promotion);
    bool IsPromotionValid(Promotion promotion);
    bool HasReachedMaxUses(Promotion promotion);
}
