using FiapFastFoodAutenticacao.Dtos;

namespace FiapFastFoodAutenticacao.Contracts;

public interface IAuthService
{
    Task<TokenResponse> AutenticacaoAdminAsync(AdminLoginRequest request);
    Task<TokenResponse> AutenticacaoTotemAsync(TotemIdentifyRequest request);
}
