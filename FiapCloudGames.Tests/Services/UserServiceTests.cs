using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Services;
using FiapCloudGames.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FiapCloudGames.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserDomainService> _userDomainServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userDomainServiceMock = new Mock<IUserDomainService>();
        _configurationMock = new Mock<IConfiguration>();
        _userService = new UserService(_userRepositoryMock.Object, _userDomainServiceMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenEmailIsUnique()
    {
        // Arrange
        var dto = new RegisterDto { Name = "Test", Email = "test@test.com", Password = "Password123!" };
        _userRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _userDomainServiceMock.Setup(s => s.HashPassword(It.IsAny<string>())).Returns("hashed_password");

        // Act
        var result = await _userService.RegisterAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(dto.Email.ToLowerInvariant());
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var dto = new RegisterDto { Name = "Test", Email = "existing@test.com", Password = "Password123!" };
        _userRepositoryMock.Setup(r => r.EmailExistsAsync(dto.Email)).ReturnsAsync(true);

        // Act
        var act = async () => await _userService.RegisterAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Email já cadastrado");
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var dto = new LoginDto { Email = "test@test.com", Password = "Password123!" };
        var user = new User { Email = dto.Email, PasswordHash = "hashed", Role = "User" };
        
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userDomainServiceMock.Setup(s => s.VerifyPassword(dto.Password, user.PasswordHash)).Returns(true);
        
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("super_secret_key_123_456_789_000");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("FiapCloudGames");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("Users");

        // Act
        var result = await _userService.AuthenticateAsync(dto);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        var dto = new LoginDto { Email = "test@test.com", Password = "wrong_password" };
        var user = new User { Email = dto.Email, PasswordHash = "hashed" };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userDomainServiceMock.Setup(s => s.VerifyPassword(dto.Password, user.PasswordHash)).Returns(false);

        // Act
        var result = await _userService.AuthenticateAsync(dto);

        // Assert
        result.Should().BeNull();
    }
}
