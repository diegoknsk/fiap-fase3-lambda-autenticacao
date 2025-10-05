namespace FiapFastFoodAutenticacao.Core.Models;

public class AdminLoginResponse
{
    public bool Success { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public string AccessToken { get; set; } = string.Empty;
    public string? IdToken { get; set; }
    public int ExpiresIn { get; set; }
    public string Message { get; set; } = string.Empty;
    
    // Manter compatibilidade com código existente
    public string Token 
    { 
        get => AccessToken; 
        set => AccessToken = value; 
    }
}

