using FiapFastFoodAutenticacao.Contracts;
using FiapFastFoodAutenticacao.Core.Repositories;
using FiapFastFoodAutenticacao.Dtos;

namespace FiapFastFoodAutenticacao.Core.UseCases;

public class CustomerRegisterUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenService _tokenService;

    public CustomerRegisterUseCase(IUsuarioRepository usuarioRepository, ITokenService tokenService)
    {
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
    }

    public async Task<CustomerTokenResponseModel> ExecuteAsync(CustomerRegisterModel request)
    {
        // Verifica se já existe usuário com este CPF
        var existingUsuario = await _usuarioRepository.ObterPorCpfAsync(request.Cpf);
        if (existingUsuario != null)
        {
            // Se já existe, retorna token para o usuário existente
            var token = _tokenService.GenerateToken(existingUsuario.Id, out var expiresAt);
            return new CustomerTokenResponseModel
            {
                Token = token,
                CustomerId = existingUsuario.Id,
                ExpiresAt = expiresAt
            };
        }

        // Cria novo usuário (mock - em produção seria salvo no banco)
        var newUsuario = new Core.Models.Usuario
        {
            Id = Guid.NewGuid(),
            Nome = request.Name,
            Email = request.Email,
            Cpf = request.Cpf,
            Senha = "1234" // Mock - em produção seria hash da senha
        };

        // Gera token JWT
        var newToken = _tokenService.GenerateToken(newUsuario.Id, out var newExpiresAt);

        return new CustomerTokenResponseModel
        {
            Token = newToken,
            CustomerId = newUsuario.Id,
            ExpiresAt = newExpiresAt
        };
    }
}
