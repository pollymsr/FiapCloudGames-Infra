using Xunit;
using Microsoft.EntityFrameworkCore;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using FiapCloudGames.API.Controllers;
using FiapCloudGames.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"Jwt:Key", "minha_chave_super_secreta_para_testes_12345"},
                {"Jwt:Issuer", "FiapCloudGames"},
                {"Jwt:Audience", "FiapCloudGamesUsers"}
            })
            .Build();
        return config;
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        // Arrange
        var context = GetDbContext();
        var config = GetConfiguration();
        var controller = new AuthController(context, config);
        
        var registerDto = new RegisterDto
        {
            Name = "Test User",
            Email = "test@email.com",
            Password = "Test@123"
        };

        // Act
        var result = await controller.Register(registerDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        var context = GetDbContext();
        var config = GetConfiguration();
        
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Existing",
            Email = "existing@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
            Role = "User"
        };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();
        
        var controller = new AuthController(context, config);
        
        var registerDto = new RegisterDto
        {
            Name = "New User",
            Email = "existing@email.com",
            Password = "Test@123"
        };

        // Act
        var result = await controller.Register(registerDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var context = GetDbContext();
        var config = GetConfiguration();
        
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
        
        var controller = new AuthController(context, config);
        
        var loginDto = new LoginDto
        {
            Email = "login@email.com",
            Password = "Test@123"
        };

        // Act
        var result = await controller.Login(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var context = GetDbContext();
        var config = GetConfiguration();
        
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
        
        var controller = new AuthController(context, config);
        
        var loginDto = new LoginDto
        {
            Email = "login2@email.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await controller.Login(loginDto);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}