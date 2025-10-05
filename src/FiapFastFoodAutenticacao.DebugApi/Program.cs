using FiapFastFoodAutenticacao.Contracts;
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
        "POST /autenticacaoTotem - Autenticação Totem"
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

app.MapPost("/autenticacaoTotem", async (TotemIdentifyRequest req, IAuthService svc) =>
{
    try
    {
        var token = await svc.AutenticacaoTotemAsync(req);
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
.WithName("AutenticacaoTotem")
.WithOpenApi();

app.Run();
