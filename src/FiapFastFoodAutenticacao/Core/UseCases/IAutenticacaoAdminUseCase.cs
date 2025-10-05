using FiapFastFoodAutenticacao.Core.Models;

namespace FiapFastFoodAutenticacao.Core.UseCases;

public interface IAutenticacaoAdminUseCase
{
    Task<AdminLoginResponse> AutenticarAsync(AdminLoginRequest request);
}

