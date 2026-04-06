namespace FiapCloudGames.Domain.Entities;

public class UserGame
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;
    public DateTime PurchaseDate { get; set; }
}