using FiapFastFoodAutenticacao.Contracts;
using FiapFastFoodAutenticacao.Data;
using FiapFastFoodAutenticacao.Dtos;

namespace FiapFastFoodAutenticacao.Core.UseCases;

public class CustomerRegisterUseCase
{
    private readonly ITokenService _tokenService;

    public CustomerRegisterUseCase(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<CustomerTokenResponseModel> ExecuteAsync(CustomerRegisterModel request)
    {
        var repo = new CustomerRepository();

        var exists = await repo.ExistsCpfAsync(request.Cpf);
        if (exists)
        {
            var existing = await repo.GetByCpfAsync(request.Cpf);
            if (existing == null)
                throw new InvalidOperationException("Erro interno: customer existe mas não foi encontrado");
                
            var token = _tokenService.GenerateToken(existing.Id, out var expiresAt);

            return new CustomerTokenResponseModel
            {
                Token = token,
                CustomerId = existing.Id,
                ExpiresAt = expiresAt
            };
        }

        var novo = new CustomerModel
        {
            Id = Guid.NewGuid(),
            Name = null, // Nome vazio - será preenchido posteriormente
            Email = null, // Email vazio - será preenchido posteriormente
            Cpf = request.Cpf,
            CustomerType = 1 // 1 = Registered
        };

        await repo.AddAsync(novo);

        var newToken = _tokenService.GenerateToken(novo.Id, out var newExpiresAt);

        return new CustomerTokenResponseModel
        {
            Token = newToken,
            CustomerId = novo.Id,
            ExpiresAt = newExpiresAt
        };
    }
}
