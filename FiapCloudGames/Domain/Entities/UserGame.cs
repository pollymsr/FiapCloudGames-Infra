using System.Text.Json.Serialization;

namespace FiapCloudGames.Domain.Entities;

public class UserGame
{
    public Guid UserId { get; set; }
    [JsonIgnore]
    public User User { get; set; } = null!;
    public Guid GameId { get; set; }
    [JsonIgnore]
    public Game Game { get; set; } = null!;
    public DateTime PurchaseDate { get; set; }
}