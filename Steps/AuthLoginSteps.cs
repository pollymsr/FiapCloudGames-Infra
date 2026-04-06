using TechTalk.SpecFlow;
using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.API.Controllers;
using FiapCloudGames.Infrastructure.Data;
using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Application.Services;
using FiapCloudGames.Infrastructure.Repositories;
using FiapCloudGames.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        // Configurar banco em mem�ria
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

        // Configurar services com dependency injection
        var userRepository = new UserRepository(_context);
        var userDomainService = new UserDomainService();
        var userService = new UserService(userRepository, userDomainService, _config);
        _controller = new AuthController(userService);
    }

    [Given(@"que o sistema est� rodando")]
    public void GivenQueOSistemaEstaRodando()
    {
        Assert.NotNull(_context);
        Assert.NotNull(_controller);
    }

    [Given(@"o banco de dados est� configurado")]
    public void GivenOBancoDeDadosEstaConfigurado()
    {
        Assert.True(_context.Database.CanConnect());
    }

    [Given(@"que existe um usu�rio cadastrado com email ""(.*)"" e senha ""(.*)""")]
    public async Task GivenQueExisteUmUsuarioCadastrado(string email, string password)
    {
        var registerDto = new RegisterDto
        {
            Name = "Usu�rio Teste",
            Email = email,
            Password = password
        };

        await _controller.Register(registerDto);
    }

    [Given(@"que não existe um usuário com email ""(.*)""")]
    public void GivenQueNaoExisteUmUsuarioComEmail(string email)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email.ToLowerInvariant());
        Assert.Null(user);
    }

    [When(@"eu fa�o login com email ""(.*)"" e senha ""(.*)""")]
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

        if (_result is CreatedAtActionResult)
            _statusCode = 201;
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

    [Then(@"o status code � (.*)")]
    public void ThenOStatusCodeE(string statusCodeStr)
    {
        int expectedStatusCode = statusCodeStr switch
        {
            "200 OK" => 200,
            "201 Created" => 201,
            "400 Bad Request" => 400,
            "401 Unauthorized" => 401,
            _ => 500
        };

        Assert.Equal(expectedStatusCode, _statusCode);
    }

    [Then(@"o usuário é criado com sucesso")]
    public void ThenOUsuarioECriadoComSucesso()
    {
        Assert.IsType<CreatedAtActionResult>(_result);
    }

    [Then(@"recebo uma mensagem de erro ""(.*)""")]
    public void ThenReceboUmaMensagemDeErro(string message)
    {
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(_result);
        Assert.Equal(message, badRequestResult.Value);
    }
}