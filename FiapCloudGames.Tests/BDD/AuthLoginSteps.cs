using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.Services;
using FiapCloudGames.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using TechTalk.SpecFlow;
using Xunit;

namespace FiapCloudGames.Tests.BDD;

[Binding]
public class AuthLoginSteps
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserDomainService> _userDomainServiceMock;
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;

    private User? _existingUser;
    private string? _generatedToken;
    private string? _emailAttempt;
    private string? _passwordAttempt;

    public AuthLoginSteps()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userDomainServiceMock = new Mock<IUserDomainService>();

        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "uma-chave-super-segura-com-32-caracteres-ou-mais" },
            { "Jwt:Issuer", "FiapCloudGames" },
            { "Jwt:Audience", "FiapCloudGamesUsers" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _userService = new UserService(
            _userRepositoryMock.Object,
            _userDomainServiceMock.Object,
            _configuration);
    }

    [Given(@"que existe um usuário com email ""(.*)"" e senha ""(.*)""")]
    public void GivenQueExisteUmUsuarioComEmailESenha(string email, string senha)
    {
        _existingUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Usuário Teste",
            Email = email.ToLowerInvariant(),
            PasswordHash = "hash-fake",
            Role = "User"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email.ToLowerInvariant()))
            .ReturnsAsync(_existingUser);

        _userDomainServiceMock
            .Setup(d => d.VerifyPassword(senha, _existingUser.PasswordHash))
            .Returns(true);

        _userDomainServiceMock
            .Setup(d => d.VerifyPassword(It.Is<string>(p => p != senha), _existingUser.PasswordHash))
            .Returns(false);
    }

    [When(@"eu solicitar autenticação com email ""(.*)"" e senha ""(.*)""")]
    public async Task WhenEuSolicitarAutenticacaoComEmailESenha(string email, string senha)
    {
        _emailAttempt = email;
        _passwordAttempt = senha;

        var dto = new LoginDto
        {
            Email = email,
            Password = senha
        };

        _generatedToken = await _userService.AuthenticateAsync(dto);
    }

    [Then(@"o sistema deve retornar um token JWT")]
    public void ThenOSistemaDeveRetornarUmTokenJwt()
    {
        Assert.False(string.IsNullOrWhiteSpace(_generatedToken));
    }

    [Then(@"o sistema não deve autenticar o usuário")]
    public void ThenOSistemaNaoDeveAutenticarOUsuario()
    {
        Assert.Null(_generatedToken);
    }
}