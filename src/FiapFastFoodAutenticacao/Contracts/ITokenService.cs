namespace FiapFastFoodAutenticacao.Contracts;

public interface ITokenService
{
    string GenerateToken(Guid customerId, out DateTime expiresAt);
}
