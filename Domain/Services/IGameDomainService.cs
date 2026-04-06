using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Services;

public interface IGameDomainService
{
    void ValidateGame(Game game);
}
