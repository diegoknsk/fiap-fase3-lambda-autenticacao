using FiapFastFoodAutenticacao.Contracts;
using FiapFastFoodAutenticacao.Dtos;
using FiapFastFoodAutenticacao.Services;
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

        // Teste 1: Autenticação Admin - Credenciais válidas
        Console.WriteLine("🔐 Teste 1: Autenticação Admin - Credenciais válidas");
        await TestAdminAuthValid(authService);

        Console.WriteLine("\n" + new string('-', 60) + "\n");

        // Teste 2: Autenticação Admin - Credenciais inválidas
        Console.WriteLine("❌ Teste 2: Autenticação Admin - Credenciais inválidas");
        await TestAdminAuthInvalid(authService);

        Console.WriteLine("\n" + new string('-', 60) + "\n");

        // Teste 3: Autenticação Totem - CPF válido
        Console.WriteLine("🔐 Teste 3: Autenticação Totem - CPF válido");
        await TestTotemAuthValid(authService);

        Console.WriteLine("\n" + new string('-', 60) + "\n");

        // Teste 4: Autenticação Totem - CPF inválido
        Console.WriteLine("❌ Teste 4: Autenticação Totem - CPF inválido");
        await TestTotemAuthInvalid(authService);

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

    static async Task TestTotemAuthValid(IAuthService authService)
    {
        var request = new TotemIdentifyRequest
        {
            Cpf = "12345678901"
        };

        try
        {
            var response = await authService.AutenticacaoTotemAsync(request);
            Console.WriteLine($"✅ Sucesso: {JsonSerializer.Serialize(response, JsonOptions)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro: {ex.Message}");
        }
    }

    static async Task TestTotemAuthInvalid(IAuthService authService)
    {
        var request = new TotemIdentifyRequest
        {
            Cpf = "99999999999" // CPF que não existe no mock
        };

        try
        {
            var response = await authService.AutenticacaoTotemAsync(request);
            Console.WriteLine($"✅ Resposta: {JsonSerializer.Serialize(response, JsonOptions)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erro esperado: {ex.Message}");
        }
    }
}