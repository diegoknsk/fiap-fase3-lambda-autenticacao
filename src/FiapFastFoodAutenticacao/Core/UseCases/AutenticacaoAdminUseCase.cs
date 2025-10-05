using FiapFastFoodAutenticacao.Core.Models;

namespace FiapFastFoodAutenticacao.Core.UseCases;

public class AutenticacaoAdminUseCase : IAutenticacaoAdminUseCase
{
    public Task<AdminLoginResponse> AutenticarAsync(AdminLoginRequest request)
    {
        // Mock: valida credenciais fixas
        if (request.Username == "admin" && request.Password == "fiap@2025")
        {
            var response = new AdminLoginResponse
            {
                Success = true,
                Token = "MOCK_ADMIN_JWT",
                Message = "ok"
            };
            return Task.FromResult(response);
        }

        var errorResponse = new AdminLoginResponse
        {
            Success = false,
            Token = string.Empty,
            Message = "Credenciais inválidas"
        };
        return Task.FromResult(errorResponse);
    }
}

