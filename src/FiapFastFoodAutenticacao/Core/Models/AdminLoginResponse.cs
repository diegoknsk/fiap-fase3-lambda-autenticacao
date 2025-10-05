namespace FiapFastFoodAutenticacao.Core.Models;

public class AdminLoginResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

