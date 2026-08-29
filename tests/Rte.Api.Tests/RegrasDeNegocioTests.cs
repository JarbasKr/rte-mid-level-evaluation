using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Rte.Api.DTOs.Auth;
using Rte.Api.DTOs.Colaboradores;
using Rte.Api.DTOs.Unidades;
using Rte.Api.DTOs.Usuarios;
using Rte.Api.Entities;

namespace Rte.Api.Tests;

public class RegrasDeNegocioTests : IClassFixture<CustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    private readonly HttpClient _client;

    public RegrasDeNegocioTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_RetornaBearerToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Login = "admin",
            Senha = "Admin@123"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        body.Should().NotBeNull();
        body!.Token.Should().NotBeNullOrWhiteSpace();
        body.Tipo.Should().Be("Bearer");
        body.Expiration.Should().BeAfter(DateTime.UtcNow);
        body.User.Should().NotBeNull();
        body.User.Name.Should().Be("admin");
    }

    [Fact]
    public async Task ObterColaboradorPorCodigo_DeveRetornarRegistro()
    {
        var token = await ObterTokenAsync();
        Autenticar(token);

        var response = await _client.GetAsync("/api/colaboradores/COL001");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ColaboradorResponse>(JsonOptions);
        body!.Codigo.Should().Be("COL001");
    }

    [Fact]
    public async Task Login_ComSenhaInvalida_Retorna401()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Login = "admin",
            Senha = "errada"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CriarColaborador_EmUnidadeInativa_DeveSerRejeitado()
    {
        var token = await ObterTokenAsync();
        Autenticar(token);

        var unidades = await _client.GetFromJsonAsync<List<UnidadeResponse>>("/api/unidades", JsonOptions);
        var inativa = unidades!.Single(x => x.Codigo == "UND002");

        var usuario = await CriarUsuarioAsync("USR-INAT-UND", "user.inativa");

        var response = await _client.PostAsJsonAsync("/api/colaboradores", new CreateColaboradorRequest
        {
            Codigo = "COL-INAT",
            Nome = "Tentativa Unidade Inativa",
            UnidadeId = inativa.Id,
            UsuarioId = usuario.Id
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CriarUsuario_ComCodigoDuplicado_DeveSerRejeitado()
    {
        var token = await ObterTokenAsync();
        Autenticar(token);

        var payload = new CreateUsuarioRequest
        {
            Codigo = "DUP001",
            Login = "user.dup.a",
            Senha = "Senha@123",
            Status = StatusRegistro.Ativo
        };

        var primeiro = await _client.PostAsJsonAsync("/api/usuarios", payload);
        primeiro.StatusCode.Should().Be(HttpStatusCode.Created);

        payload.Login = "user.dup.b";
        var segundo = await _client.PostAsJsonAsync("/api/usuarios", payload);
        segundo.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task EndpointsProtegidos_SemToken_Retornam401()
    {
        var response = await _client.GetAsync("/api/usuarios");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<string> ObterTokenAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Login = "admin",
            Senha = "Admin@123"
        });

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        return body!.Token;
    }

    private void Autenticar(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<UsuarioResponse> CriarUsuarioAsync(string codigo, string login)
    {
        var response = await _client.PostAsJsonAsync("/api/usuarios", new CreateUsuarioRequest
        {
            Codigo = codigo,
            Login = login,
            Senha = "Senha@123",
            Status = StatusRegistro.Ativo
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<UsuarioResponse>(JsonOptions))!;
    }
}
