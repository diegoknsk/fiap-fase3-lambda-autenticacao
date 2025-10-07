using FiapFastFoodAutenticacao.Contracts;
using FiapFastFoodAutenticacao.Data;
using FiapFastFoodAutenticacao.Dtos;

namespace FiapFastFoodAutenticacao.Core.UseCases;

public class CustomerIdentifyUseCase
{
    private readonly ITokenService _tokenService;

    public CustomerIdentifyUseCase(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<CustomerTokenResponseModel> ExecuteAsync(string cpf)
    {
        var repo = new CustomerRepository();
        var usuario = await repo.GetByCpfAsync(cpf);

        if (usuario == null)
            throw new UnauthorizedAccessException("CPF não encontrado");

        var token = _tokenService.GenerateToken(usuario.Id, out var expiresAt);

        return new CustomerTokenResponseModel
        {
            Token = token,
            CustomerId = usuario.Id,
            ExpiresAt = expiresAt
        };
    }
}
