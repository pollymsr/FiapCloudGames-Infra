using Xunit;
using Microsoft.EntityFrameworkCore;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Infrastructure.Data;
using FiapCloudGames.Infrastructure.Repositories;
using FiapCloudGames.Domain.Services;
using FiapCloudGames.Application.Services;
using FiapCloudGames.API.Controllers;
using FiapCloudGames.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Tests.Controllers;

public class UserControllerTests
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

    private UserController GetUserController(AppDbContext context)
    {
        var userService = new UserService(new UserRepository(context), new UserDomainService(), GetConfiguration());
        return new UserController(userService);
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

    [Fact]
    public async Task GetAll_ReturnsUserList()
    {
        var context = GetDbContext();
        context.Users.Add(new User { Id = Guid.NewGuid(), Name = "User A", Email = "a@email.com", PasswordHash = "hash", Role = "User" });
        await context.SaveChangesAsync();

        var controller = GetUserController(context);
        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var users = Assert.IsAssignableFrom<IEnumerable<UserResponseDto>>(okResult.Value);
        Assert.Single(users);
    }

    [Fact]
    public async Task ChangeRole_WithValidRole_ReturnsNoContent()
    {
        var context = GetDbContext();
        var user = new User { Id = Guid.NewGuid(), Name = "User B", Email = "b@email.com", PasswordHash = "hash", Role = "User" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = GetUserController(context);
        var result = await controller.ChangeRole(user.Id, new ChangeUserRoleDto { Role = "Admin" });

        Assert.IsType<NoContentResult>(result);
        var updatedUser = await context.Users.FindAsync(user.Id);
        Assert.Equal("Admin", updatedUser!.Role);
    }

    [Fact]
    public async Task Delete_WithExistingUser_ReturnsNoContent()
    {
        var context = GetDbContext();
        var user = new User { Id = Guid.NewGuid(), Name = "User C", Email = "c@email.com", PasswordHash = "hash", Role = "User" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = GetUserController(context);
        var result = await controller.Delete(user.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(await context.Users.FindAsync(user.Id));
    }
}
