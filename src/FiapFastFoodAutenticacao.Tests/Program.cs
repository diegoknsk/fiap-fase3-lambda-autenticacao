using FiapFastFoodAutenticacao.Contracts;
using FiapFastFoodAutenticacao.Core.Repositories;
using FiapFastFoodAutenticacao.Core.UseCases;
using FiapFastFoodAutenticacao.Dtos;
using FiapFastFoodAutenticacao.Services;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace FiapFastFoodAutenticacao.Tests;

class Program
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Teste dos UseCases/Services do Core ===\n");
        Console.WriteLine("Testando a lógica de negócio concentrada no projeto principal...\n");

        var authService = new AuthService();
        
        // Configuração para o TokenService
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "FiapFastFoodSuperSecretKeyForJWTTokenGeneration2025!",
                ["JwtSettings:Issuer"] = "FiapFastFood",
                ["JwtSettings:Audience"] = "FiapFastFood",
                ["JwtSettings:ExpirationHours"] = "3"
            })
            .Build();
            
        var usuarioRepository = new UsuarioRepositoryMock();
        var tokenService = new TokenService(configuration);
        var customerUseCase = new CustomerUseCase(usuarioRepository, tokenService);

        // Teste 1: Autenticação Admin - Credenciais válidas
        Console.WriteLine("🔐 Teste 1: Autenticação Admin - Credenciais válidas");
        await TestAdminAuthValid(authService);

        Console.WriteLine("\n" + new string('-', 60) + "\n");

        // Teste 2: Autenticação Admin - Credenciais inválidas
        Console.WriteLine("❌ Teste 2: Autenticação Admin - Credenciais inválidas");
        await TestAdminAuthInvalid(authService);

        Console.WriteLine("\n" + new string('-', 60) + "\n");

        // Teste 3: Customer Identify - CPF válido
        Console.WriteLine("🔐 Teste 3: Customer Identify - CPF válido");
        await TestCustomerIdentifyValid(customerUseCase);

        Console.WriteLine("\n" + new string('-', 60) + "\n");

        // Teste 4: Customer Identify - CPF inválido
        Console.WriteLine("❌ Teste 4: Customer Identify - CPF inválido");
        await TestCustomerIdentifyInvalid(customerUseCase);

        Console.WriteLine("\n" + new string('-', 60) + "\n");

        // Teste 5: Customer Register - Dados válidos
        Console.WriteLine("🔐 Teste 5: Customer Register - Dados válidos");
        await TestCustomerRegisterValid(customerUseCase);

        Console.WriteLine("\n" + new string('-', 60) + "\n");

        // Teste 6: Customer Register Anonymous
        Console.WriteLine("🔐 Teste 6: Customer Register Anonymous");
        await TestCustomerRegisterAnonymous(customerUseCase);

        Console.WriteLine("\n=== Testes Concluídos ===");
        Console.WriteLine("Pressione qualquer tecla para sair...");
        Console.ReadKey();
    }

    static async Task TestAdminAuthValid(IAuthService authService)
    {
        var request = new AdminLoginRequest
        {
            Email = "admin@fiap.com",
            Password = "fiap@2025"
        };

        try
        {
            var response = await authService.AutenticacaoAdminAsync(request);
            Console.WriteLine($"✅ Sucesso: {JsonSerializer.Serialize(response, JsonOptions)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro: {ex.Message}");
        }
    }

    static async Task TestAdminAuthInvalid(IAuthService authService)
    {
        var request = new AdminLoginRequest
        {
            Email = "admin@fiap.com",
            Password = "senha_errada"
        };

        try
        {
            var response = await authService.AutenticacaoAdminAsync(request);
            Console.WriteLine($"✅ Resposta: {JsonSerializer.Serialize(response, JsonOptions)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro esperado: {ex.Message}");
        }
    }

    static async Task TestCustomerIdentifyValid(ICustomerUseCase customerUseCase)
    {
        try
        {
            var response = await customerUseCase.IdentifyAsync("12345678901");
            Console.WriteLine($"✅ Sucesso: {JsonSerializer.Serialize(response, JsonOptions)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro: {ex.Message}");
        }
    }

    static async Task TestCustomerIdentifyInvalid(ICustomerUseCase customerUseCase)
    {
        try
        {
            var response = await customerUseCase.IdentifyAsync("99999999999");
            Console.WriteLine($"✅ Resposta: {JsonSerializer.Serialize(response, JsonOptions)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro esperado: {ex.Message}");
        }
    }

    static async Task TestCustomerRegisterValid(ICustomerUseCase customerUseCase)
    {
        var request = new CustomerRegisterModel
        {
            Name = "João Silva Teste",
            Email = "joao.teste@email.com",
            Cpf = "98765432100"
        };

        try
        {
            var response = await customerUseCase.RegisterAsync(request);
            Console.WriteLine($"✅ Sucesso: {JsonSerializer.Serialize(response, JsonOptions)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro: {ex.Message}");
        }
    }

    static async Task TestCustomerRegisterAnonymous(ICustomerUseCase customerUseCase)
    {
        try
        {
            var response = await customerUseCase.RegisterAnonymousAsync();
            Console.WriteLine($"✅ Sucesso: {JsonSerializer.Serialize(response, JsonOptions)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro: {ex.Message}");
        }
    }
}