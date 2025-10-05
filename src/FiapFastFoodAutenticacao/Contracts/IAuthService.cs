using FiapFastFoodAutenticacao.Dtos;

namespace FiapFastFoodAutenticacao.Contracts;

public interface IAuthService
{
    Task<TokenResponse> AutenticacaoAdminAsync(AdminLoginRequest request);
}
