using FiapFastFoodAutenticacao.Core.Models;

namespace FiapFastFoodAutenticacao.Core.UseCases;

public interface IAutenticacaoTotemUseCase
{
    Task<TotemLoginResponse> AutenticarAsync(TotemLoginRequest request);
}

