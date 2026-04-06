using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Application.DTOs;

public class CreateGameDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public string Genre { get; set; } = string.Empty;

    [Required]
    public DateTime ReleaseDate { get; set; }
}

public class UpdateGameDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    public string Genre { get; set; } = string.Empty;

    [Required]
    public DateTime ReleaseDate { get; set; }
}
