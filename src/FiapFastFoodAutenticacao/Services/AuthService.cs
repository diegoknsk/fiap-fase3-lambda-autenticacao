using FiapFastFoodAutenticacao.Contracts;
using FiapFastFoodAutenticacao.Dtos;

namespace FiapFastFoodAutenticacao.Services;

public class AuthService : IAuthService
{
    public async Task<TokenResponse> AutenticacaoAdminAsync(AdminLoginRequest request)
    {
        // Simular delay de rede/banco
        await Task.Delay(100);

        // Mock: credenciais válidas
        if (request.Email == "admin@fiap.com" && request.Password == "fiap@2025")
        {
            return new TokenResponse
            {
                Token = "MOCK_ADMIN_JWT_TOKEN",
                ExpiresAt = DateTime.UtcNow.AddHours(8)
            };
        }

        throw new UnauthorizedAccessException("Credenciais inválidas para admin");
    }

    public async Task<TokenResponse> AutenticacaoTotemAsync(TotemIdentifyRequest request)
    {
        // Simular delay de rede/banco
        await Task.Delay(100);

        // Mock: CPF válido
        if (request.Cpf == "12345678901")
        {
            return new TokenResponse
            {
                Token = "MOCK_TOTEM_JWT_TOKEN",
                ExpiresAt = DateTime.UtcNow.AddHours(4)
            };
        }

        throw new UnauthorizedAccessException("CPF não encontrado ou inválido");
    }
}
