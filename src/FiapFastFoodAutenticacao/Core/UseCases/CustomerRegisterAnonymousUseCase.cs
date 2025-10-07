using FiapFastFoodAutenticacao.Contracts;
using FiapFastFoodAutenticacao.Data;
using FiapFastFoodAutenticacao.Dtos;

namespace FiapFastFoodAutenticacao.Core.UseCases;

public class CustomerRegisterAnonymousUseCase
{
    private readonly ITokenService _tokenService;

    public CustomerRegisterAnonymousUseCase(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<CustomerTokenResponseModel> ExecuteAsync()
    {
        var repo = new CustomerRepository();

        var anonymous = new CustomerModel
        {
            Id = Guid.NewGuid(),
            Name = null, // Nome vazio - usuário anônimo
            Email = null, // Email vazio - usuário anônimo
            Cpf = null, // CPF vazio - usuário anônimo
            CustomerType = 2 // 2 = Anonymous
        };

        await repo.AddAsync(anonymous);

        var token = _tokenService.GenerateToken(anonymous.Id, out var expiresAt);

        return new CustomerTokenResponseModel
        {
            Token = token,
            CustomerId = anonymous.Id,
            ExpiresAt = expiresAt
        };
    }
}
