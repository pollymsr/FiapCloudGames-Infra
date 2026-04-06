using TechTalk.SpecFlow;
using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.API.Controllers;
using FiapCloudGames.Infrastructure.Data;
using FiapCloudGames.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using Xunit;

namespace FiapCloudGames.Tests.Steps;

[Binding]
public class AuthLoginSteps
{
    private readonly ScenarioContext _scenarioContext;
    private AppDbContext _context;
    private IConfiguration _config;
    private AuthController _controller;
    private IActionResult _result;
    private string _token;
    private int _statusCode;

    public AuthLoginSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;

        // Configurar banco em memória
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        // Configurar JWT
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"Jwt:Key", "chave_super_secreta_para_testes_bdd_1234567890"},
                {"Jwt:Issuer", "FiapCloudGames"},
                {"Jwt:Audience", "FiapCloudGamesUsers"}
            })
            .Build();

        _config = configuration;
        _controller = new AuthController(_context, _config);
    }

    [Given(@"que o sistema está rodando")]
    public void GivenQueOSistemaEstaRodando()
    {
        Assert.NotNull(_context);
        Assert.NotNull(_controller);
    }

    [Given(@"o banco de dados está configurado")]
    public void GivenOBancoDeDadosEstaConfigurado()
    {
        Assert.True(_context.Database.CanConnect());
    }

    [Given(@"que existe um usuário cadastrado com email ""(.*)"" e senha ""(.*)""")]
    public async Task GivenQueExisteUmUsuarioCadastrado(string email, string password)
    {
        var registerDto = new RegisterDto
        {
            Name = "Usuário Teste",
            Email = email,
            Password = password
        };

        await _controller.Register(registerDto);
    }

    [Given(@"que não existe um usuário com email ""(.*)""")]
    public void GivenQueNaoExisteUmUsuarioComEmail(string email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        Assert.Null(user);
    }

    [When(@"eu faço login com email ""(.*)"" e senha ""(.*)""")]
    public async Task WhenEuFacoLogin(string email, string password)
    {
        var loginDto = new LoginDto
        {
            Email = email,
            Password = password
        };

        _result = await _controller.Login(loginDto);

        if (_result is OkObjectResult okResult && okResult.Value != null)
        {
            var json = JObject.Parse(okResult.Value.ToString()!);
            _token = json["token"]?.ToString();
            _statusCode = 200;
        }
        else if (_result is UnauthorizedObjectResult unauthorizedResult)
        {
            _statusCode = 401;
        }
    }

    [When(@"eu registro um novo usuário com nome ""(.*)"", email ""(.*)"" e senha ""(.*)""")]
    public async Task WhenEuRegistroUmNovoUsuario(string name, string email, string password)
    {
        var registerDto = new RegisterDto
        {
            Name = name,
            Email = email,
            Password = password
        };

        _result = await _controller.Register(registerDto);

        if (_result is OkObjectResult)
            _statusCode = 200;
        else if (_result is BadRequestObjectResult)
            _statusCode = 400;
    }

    [Then(@"eu recebo um token JWT válido")]
    public void ThenEuReceboUmTokenJWTValido()
    {
        Assert.NotNull(_token);
        Assert.NotEmpty(_token);
    }

    [Then(@"o token contém o papel ""(.*)""")]
    public void ThenOTokenContemOPapel(string role)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(_token);

        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        Assert.NotNull(roleClaim);
        Assert.Equal(role, roleClaim.Value);
    }

    [Then(@"eu recebo uma resposta ""(.*)""")]
    public void ThenEuReceboUmaResposta(string message)
    {
        if (_result is UnauthorizedObjectResult unauthorizedResult)
        {
            Assert.Equal(message, unauthorizedResult.Value);
        }
        else if (_result is BadRequestObjectResult badRequestResult)
        {
            Assert.Equal(message, badRequestResult.Value);
        }
    }

    [Then(@"o status code é (.*)")]
    public void ThenOStatusCodeE(int statusCode)
    {
        Assert.Equal(statusCode, _statusCode);
    }

    [Then(@"o usuário é criado com sucesso")]
    public void ThenOUsuarioECriadoComSucesso()
    {
        Assert.IsType<OkObjectResult>(_result);
    }

    [Then(@"recebo uma mensagem de erro ""(.*)""")]
    public void ThenReceboUmaMensagemDeErro(string message)
    {
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(_result);
        Assert.Equal(message, badRequestResult.Value);
    }
}