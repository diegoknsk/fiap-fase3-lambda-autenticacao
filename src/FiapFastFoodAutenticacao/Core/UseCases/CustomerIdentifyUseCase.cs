using FiapFastFoodAutenticacao.Contracts;
using FiapFastFoodAutenticacao.Core.Repositories;
using FiapFastFoodAutenticacao.Dtos;

namespace FiapFastFoodAutenticacao.Core.UseCases;

public class CustomerIdentifyUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenService _tokenService;

    public CustomerIdentifyUseCase(IUsuarioRepository usuarioRepository, ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
    }

    public async Task<CustomerTokenResponseModel> ExecuteAsync(string cpf)
    {
        // Busca usuário pelo CPF
        var usuario = await _usuarioRepository.ObterPorCpfAsync(cpf);
        
        if (usuario == null)
        {
            throw new UnauthorizedAccessException("CPF não encontrado");
        }

        // Gera token JWT
        var token = _tokenService.GenerateToken(usuario.Id, out var expiresAt);

        return new CustomerTokenResponseModel
        {
            Token = token,
            CustomerId = usuario.Id,
            ExpiresAt = expiresAt
        };
    }
}
