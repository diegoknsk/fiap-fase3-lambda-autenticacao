namespace FiapFastFoodAutenticacao.Dtos;

public class AdminLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class TotemIdentifyRequest
{
    public string Cpf { get; set; } = string.Empty;
}
