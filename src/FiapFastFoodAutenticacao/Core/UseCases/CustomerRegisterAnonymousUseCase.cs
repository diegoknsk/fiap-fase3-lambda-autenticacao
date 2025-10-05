using FiapFastFoodAutenticacao.Contracts;
using FiapFastFoodAutenticacao.Core.Repositories;
using FiapFastFoodAutenticacao.Dtos;

namespace FiapFastFoodAutenticacao.Core.UseCases;

public class CustomerRegisterAnonymousUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenService _tokenService;

    public CustomerRegisterAnonymousUseCase(IUsuarioRepository usuarioRepository, ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
    }

    public async Task<CustomerTokenResponseModel> ExecuteAsync()
    {
        // Cria usuário anônimo (mock - em produção seria salvo no banco)
        var anonymousUsuario = new Core.Models.Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Usuário Anônimo",
            Email = $"anonymous_{Guid.NewGuid()}@temp.com",
            Cpf = $"anonymous_{Guid.NewGuid()}",
            Senha = "anonymous"
        };

        // Gera token JWT
        var token = _tokenService.GenerateToken(anonymousUsuario.Id, out var expiresAt);

        return new CustomerTokenResponseModel
        {
            Token = token,
            CustomerId = anonymousUsuario.Id,
            ExpiresAt = expiresAt
        };
    }
}
