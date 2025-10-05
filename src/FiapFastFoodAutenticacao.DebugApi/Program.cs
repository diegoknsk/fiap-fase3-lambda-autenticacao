using FiapFastFoodAutenticacao.Contracts;
using FiapFastFoodAutenticacao.Core.Repositories;
using FiapFastFoodAutenticacao.Core.UseCases;
using FiapFastFoodAutenticacao.Dtos;
using FiapFastFoodAutenticacao.Services;

var builder = WebApplication.CreateBuilder(args);

// Opcional: DI só aqui (camada de host)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "FiapFastFood Debug API", 
        Version = "v1",
        Description = "Host de depuração - lógica concentrada no projeto Core"
    });
});

// DI limitado ao DebugApi para injetar IAuthService
builder.Services.AddSingleton<IAuthService, AuthService>(); // atual: mock; depois troca pelo real

// DI para Customer endpoints
builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepositoryMock>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<CustomerIdentifyUseCase>();
builder.Services.AddSingleton<CustomerRegisterUseCase>();
builder.Services.AddSingleton<CustomerRegisterAnonymousUseCase>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FiapFastFood Debug API v1");
    c.RoutePrefix = string.Empty; // Para acessar o Swagger na raiz
});

// Endpoint de status
app.MapGet("/", () => new { 
    Message = "FiapFastFood Debug API - Host de Depuração", 
    Version = "1.0.0",
    Description = "Esta API é apenas para debug local. A lógica de autenticação está 100% concentrada no projeto FiapFastFoodAutenticacao. Produção roda apenas a função Lambda.",
    Endpoints = new[] {
        "POST /autenticacaoAdmin - Autenticação Admin",
        "POST /api/customer/identify - Identificar Customer por CPF",
        "POST /api/customer/register - Registrar Customer",
        "POST /api/customer/anonymous - Registrar Customer Anônimo"
    }
});

// NENHUMA regra aqui. Apenas delega para os UseCases/Services do Core
app.MapPost("/autenticacaoAdmin", async (AdminLoginRequest req, IAuthService svc) =>
{
    try
    {
        var token = await svc.AutenticacaoAdminAsync(req);
        return Results.Ok(token);
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Unauthorized();
    }
    catch (Exception)
    {
        return Results.Problem("Erro interno do servidor");
    }
})
.WithName("AutenticacaoAdmin")
.WithOpenApi();

// Customer endpoints
app.MapPost("/api/customer/identify", async (CustomerIdentifyModel req, CustomerIdentifyUseCase identifyUseCase) =>
{
    try
    {
        var result = await identifyUseCase.ExecuteAsync(req.Cpf);
        return Results.Ok(ApiResponse<CustomerTokenResponseModel>.Ok(result));
    }
    catch (UnauthorizedAccessException ex)
    {
        return Results.Ok(ApiResponse<CustomerTokenResponseModel>.Error(ex.Message));
    }
    catch (Exception)
    {
        return Results.Problem("Erro interno do servidor");
    }
})
.WithName("CustomerIdentify")
.WithOpenApi();

app.MapPost("/api/customer/register", async (CustomerRegisterModel req, CustomerRegisterUseCase registerUseCase) =>
{
    try
    {
        var result = await registerUseCase.ExecuteAsync(req);
        return Results.Ok(ApiResponse<CustomerTokenResponseModel>.Ok(result));
    }
    catch (Exception)
    {
        return Results.Problem("Erro interno do servidor");
    }
})
.WithName("CustomerRegister")
.WithOpenApi();

app.MapPost("/api/customer/anonymous", async (CustomerRegisterAnonymousUseCase registerAnonymousUseCase) =>
{
    try
    {
        var result = await registerAnonymousUseCase.ExecuteAsync();
        return Results.Ok(ApiResponse<CustomerTokenResponseModel>.Ok(result));
    }
    catch (Exception)
    {
        return Results.Problem("Erro interno do servidor");
    }
})
.WithName("CustomerAnonymous")
.WithOpenApi();


app.Run();