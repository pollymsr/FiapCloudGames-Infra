using Xunit;
using System.ComponentModel.DataAnnotations;
using FiapCloudGames.Application.DTOs;
using FluentAssertions;

namespace FiapCloudGames.Tests.DTOs;

public class RegisterDtoTests
{
    private static List<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true);

        return results;
    }

    [Fact]
    public void Should_Be_Valid_When_All_Register_Fields_Are_Correct()
    {
        var dto = new RegisterDto
        {
            Name = "Pollyana",
            Email = "pollyana@email.com",
            Password = "Senha@123"
        };

        var results = ValidateModel(dto);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Should_Be_Invalid_When_Email_Is_Wrong()
    {
        var dto = new RegisterDto
        {
            Name = "Pollyana",
            Email = "email-invalido",
            Password = "Senha@123"
        };

        var results = ValidateModel(dto);

        results.Should().NotBeEmpty();
        results.Should().Contain(r => r.MemberNames.Contains(nameof(RegisterDto.Email)));
    }

    [Fact]
    public void Should_Be_Invalid_When_Password_Is_Weak()
    {
        var dto = new RegisterDto
        {
            Name = "Pollyana",
            Email = "pollyana@email.com",
            Password = "12345678"
        };

        var results = ValidateModel(dto);

        results.Should().NotBeEmpty();
        results.Should().Contain(r => r.MemberNames.Contains(nameof(RegisterDto.Password)));
    }

    [Fact]
    public void Should_Be_Invalid_When_Password_Has_Less_Than_8_Characters()
    {
        var dto = new RegisterDto
        {
            Name = "Pollyana",
            Email = "pollyana@email.com",
            Password = "Ab@12"
        };

        var results = ValidateModel(dto);

        results.Should().NotBeEmpty();
        results.Should().Contain(r => r.MemberNames.Contains(nameof(RegisterDto.Password)));
    }
}