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

}
