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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Adm1n@SecurePass2026"),
                Role = "Admin"
            };
            context.Users.Add(admin);
            Console.WriteLine("Admin criado com sucesso!");
        }
        else
        {
            Console.WriteLine("Admin já existe.");
        }

        const string defaultUserEmail = "user@fiapcloudgames.com";
        var defaultUserEmailLower = defaultUserEmail.ToLowerInvariant();
        var existingUser = context.Users.FirstOrDefault(u => u.Email == defaultUserEmailLower);

        if (existingUser == null)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "FCG User",
                Email = defaultUserEmailLower,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@SecurePass2026"),
                Role = "User"
            };
            context.Users.Add(user);
            Console.WriteLine("Usuário padrão criado com sucesso!");
        }
        else
        {
            Console.WriteLine("Usuário padrão já existe.");
        }

        if (!context.Games.Any())
        {
            var games = new List<Game>
            {
                new Game
                {
                    Id = Guid.NewGuid(),
                    Title = "Code Quest",
                    Description = "A jornada para aprender lógica de programação em aventuras interativas.",
                    Genre = "Educação",
                    Price = 29.90m,
                    ReleaseDate = DateTime.UtcNow.AddMonths(-2)
                },
                new Game
                {
                    Id = Guid.NewGuid(),
                    Title = "Cloud Ops Simulator",
                    Description = "Gerencie servidores em nuvem e aprenda conceitos de infraestrutura.",
                    Genre = "Estratégia",
                    Price = 49.90m,
                    ReleaseDate = DateTime.UtcNow.AddMonths(-1)
                },
                new Game
                {
                    Id = Guid.NewGuid(),
                    Title = "Cyber Security Challenge",
                    Description = "Resolva desafios de segurança cibernética e proteja sistemas digitais.",
                    Genre = "Puzzle",
                    Price = 39.90m,
                    ReleaseDate = DateTime.UtcNow.AddMonths(-3)
                }
            };
            context.Games.AddRange(games);
            Console.WriteLine("Jogos de exemplo adicionados.");
        }
        else
        {
            Console.WriteLine("Jogos de exemplo já existem.");
        }

        context.SaveChanges();
    }
}
