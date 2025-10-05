using FiapFastFoodAutenticacao.Core.Models;

namespace FiapFastFoodAutenticacao.Core.Models;

public class TotemLoginResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Usuario? Usuario { get; set; }
}

