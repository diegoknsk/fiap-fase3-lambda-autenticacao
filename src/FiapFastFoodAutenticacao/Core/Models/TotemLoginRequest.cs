namespace FiapFastFoodAutenticacao.Core.Models;

public class TotemLoginRequest
{
    public string Cpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

