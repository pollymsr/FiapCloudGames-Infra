using Xunit;
using FiapCloudGames.API.Controllers;
using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FiapCloudGames.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new AuthController(_userServiceMock.Object);
    }

    [Fact]
    public async Task Register_Should_Return_CreatedAtAction_When_User_Is_Created()
    {
        var dto = new RegisterDto
        {
            Name = "Teste",
            Email = "teste@email.com",
            Password = "Senha@123"
        };

        _userServiceMock
            .Setup(s => s.RegisterAsync(dto))
            .ReturnsAsync(new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Role = "User"
            });

        var result = await _controller.Register(dto);

        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Register_Should_Return_BadRequest_When_Email_Already_Exists()
    {
        var dto = new RegisterDto
        {
            Name = "Teste",
            Email = "teste@email.com",
            Password = "Senha@123"
        };

        _userServiceMock
            .Setup(s => s.RegisterAsync(dto))
            .ThrowsAsync(new InvalidOperationException("Email já cadastrado"));

        var result = await _controller.Register(dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_Should_Return_Ok_When_Credentials_Are_Valid()
    {
        var dto = new LoginDto
        {
            Email = "teste@email.com",
            Password = "Senha@123"
        };

        _userServiceMock
            .Setup(s => s.AuthenticateAsync(dto))
            .ReturnsAsync("fake-jwt-token");

        var result = await _controller.Login(dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_When_Credentials_Are_Invalid()
    {
        var dto = new LoginDto
        {
            Email = "teste@email.com",
            Password = "senha-errada"
        };

        _userServiceMock
            .Setup(s => s.AuthenticateAsync(dto))
            .ReturnsAsync((string?)null);

        var result = await _controller.Login(dto);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}