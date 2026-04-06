using System.ComponentModel.DataAnnotations;

namespace FiapCloudGames.Application.DTOs;

public class RegisterDto
{
    public string Name { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [MinLength(8)]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&]).+$")]
    public string Password { get; set; }
}