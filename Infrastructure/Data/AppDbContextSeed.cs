using FiapCloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGames.Infrastructure.Data;

public static class AppDbContextSeed
{
    public static void Seed(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Database.Migrate();

        const string adminEmail = "admin@fiapcloudgames.com";
        var adminEmailLower = adminEmail.ToLowerInvariant();
        
        var existingAdmin = context.Users.FirstOrDefault(u => u.Email == adminEmailLower);
        if (existingAdmin == null)
        {
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Name = "FCG Administrator",
                Email = adminEmailLower,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin"
            };
            context.Users.Add(admin);
            context.SaveChanges();
            Console.WriteLine("Admin criado com sucesso!");
        }
        else
        {
            Console.WriteLine("Admin já existe.");
        }
    }
}
