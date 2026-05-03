using Xunit;
using System.ComponentModel.DataAnnotations;
using FiapCloudGames.Application.DTOs;
using FluentAssertions;

namespace FiapCloudGames.Tests.DTOs;

public class LoginDtoTests
{
    private static List<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true);

        return results;
    }

    [Fact]
    public void Should_Be_Valid_When_Login_Data_Is_Correct()
    {
        var dto = new LoginDto
        {
            Email = "pollyana@email.com",
            Password = "Senha@123"
        };

        var results = ValidateModel(dto);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Should_Be_Invalid_When_Email_Is_Invalid()
    {
        var dto = new LoginDto
        {
            Email = "email-invalido",
            Password = "Senha@123"
        };

        var results = ValidateModel(dto);

        results.Should().NotBeEmpty();
        results.Should().Contain(r => r.MemberNames.Contains(nameof(LoginDto.Email)));
    }

    [Fact]
    public void Should_Be_Invalid_When_Password_Is_Empty()
    {
        var dto = new LoginDto
        {
            Email = "pollyana@email.com",
            Password = ""
        };

        var results = ValidateModel(dto);

        results.Should().NotBeEmpty();
        results.Should().Contain(r => r.MemberNames.Contains(nameof(LoginDto.Password)));
    }
}