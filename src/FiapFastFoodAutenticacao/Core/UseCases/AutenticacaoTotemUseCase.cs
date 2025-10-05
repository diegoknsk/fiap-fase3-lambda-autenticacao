using FiapFastFoodAutenticacao.Core.Models;
using FiapFastFoodAutenticacao.Core.Repositories;

namespace FiapFastFoodAutenticacao.Core.UseCases;

public class AutenticacaoTotemUseCase : IAutenticacaoTotemUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AutenticacaoTotemUseCase(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<TotemLoginResponse> AutenticarAsync(TotemLoginRequest request)
    {
        // Busca usuário pelo CPF
        var usuario = await _usuarioRepository.ObterPorCpfAsync(request.Cpf);
        
        if (usuario == null)
        {
            return new TotemLoginResponse
            {
                Success = false,
                Token = string.Empty,
                Message = "Usuário não encontrado",
                Usuario = null
            };
        }

        // Mock: valida senha fixa "1234"
        if (request.Senha == "1234")
        {
            return new TotemLoginResponse
            {
                Success = true,
                Token = "MOCK_TOTEM_JWT",
                Message = "ok",
                Usuario = usuario
            };
        }

        return new TotemLoginResponse
        {
            Success = false,
            Token = string.Empty,
            Message = "Senha inválida",
            Usuario = null
        };
    }
}

