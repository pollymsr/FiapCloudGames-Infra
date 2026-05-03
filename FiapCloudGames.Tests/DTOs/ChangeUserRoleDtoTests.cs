using Xunit;
using System.ComponentModel.DataAnnotations;
using FiapCloudGames.Application.DTOs;
using FluentAssertions;

namespace FiapCloudGames.Tests.DTOs;

public class ChangeUserRoleDtoTests
{
    private static List<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, results, true);

        return results;
    }

    [Fact]
    public void Should_Be_Valid_When_Role_Is_User()
    {
        var dto = new ChangeUserRoleDto
        {
            Role = "User"
        };

        var results = ValidateModel(dto);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Should_Be_Valid_When_Role_Is_Admin()
    {
        var dto = new ChangeUserRoleDto
        {
            Role = "Admin"
        };

        var results = ValidateModel(dto);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Should_Be_Invalid_When_Role_Is_Different_From_User_Or_Admin()
    {
        var dto = new ChangeUserRoleDto
        {
            Role = "Manager"
        };

        var results = ValidateModel(dto);

        results.Should().NotBeEmpty();
        results.Should().Contain(r => r.MemberNames.Contains(nameof(ChangeUserRoleDto.Role)));
    }

    [Fact]
    public void Should_Be_Invalid_When_Role_Is_Empty()
    {
        var dto = new ChangeUserRoleDto
        {
            Role = ""
        };

        var results = ValidateModel(dto);

        results.Should().NotBeEmpty();
        results.Should().Contain(r => r.MemberNames.Contains(nameof(ChangeUserRoleDto.Role)));
    }
}