using Xunit;
using Microsoft.EntityFrameworkCore;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using FiapCloudGames.Infrastructure.Repositories;
using FiapCloudGames.Domain.Services;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Application.DTOs;
using FiapCloudGames.API.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Tests.Controllers;

public class AuthControllerTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        var context = new AppDbContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        return context;
    }

    private IConfiguration GetConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"Jwt:Key", "minha_chave_super_secreta_para_testes_12345"},
                {"Jwt:Issuer", "FiapCloudGames"},
                {"Jwt:Audience", "FiapCloudGamesUsers"}
            })
            .Build();
    }

    private IUserService GetUserService(AppDbContext context)
    {
        return new UserService(new UserRepository(context), new UserDomainService(), GetConfiguration());
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsCreated()
    {
        var context = GetDbContext();
        var controller = new AuthController(GetUserService(context));

        var registerDto = new RegisterDto
        {
            Name = "Test User",
            Email = "test@email.com",
            Password = "Test@123"
        };

        var result = await controller.Register(registerDto);

        Assert.IsType<CreatedAtActionResult>(result);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsBadRequest()
    {
        var context = GetDbContext();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Existing",
            Email = "existing@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
            Role = "User"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new AuthController(GetUserService(context));
        var registerDto = new RegisterDto
        {
            Name = "New User",
            Email = "existing@email.com",
            Password = "Test@123"
        };

        var result = await controller.Register(registerDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var context = GetDbContext();
        var config = GetConfiguration();
        var userService = new UserService(new UserRepository(context), new UserDomainService(), config);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Login User",
            Email = "login@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
            Role = "User"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new AuthController(userService);
        var loginDto = new LoginDto
        {
            Email = "login@email.com",
            Password = "Test@123"
        };

        var result = await controller.Login(loginDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        var context = GetDbContext();
        var userService = new UserService(new UserRepository(context), new UserDomainService(), GetConfiguration());

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Login User",
            Email = "login2@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
            Role = "User"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new AuthController(userService);
        var loginDto = new LoginDto
        {
            Email = "login2@email.com",
            Password = "WrongPassword"
        };

        var result = await controller.Login(loginDto);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
