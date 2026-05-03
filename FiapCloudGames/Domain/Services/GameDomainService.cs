using FiapCloudGames.Domain.Entities;

namespace FiapCloudGames.Domain.Services;

public class GameDomainService : IGameDomainService
{
    public void ValidateGame(Game game)
    {
        if (string.IsNullOrWhiteSpace(game.Title))
            throw new InvalidOperationException("O título do jogo não pode ficar vazio.");

        if (string.IsNullOrWhiteSpace(game.Description))
            throw new InvalidOperationException("A descrição do jogo não pode ficar vazia.");

        if (game.Price < 0)
            throw new InvalidOperationException("O preço do jogo deve ser maior ou igual a zero.");

        if (game.ReleaseDate > DateTime.UtcNow.AddYears(1))
            throw new InvalidOperationException("A data de lançamento não pode ser posterior a um ano no futuro.");
    }
}
