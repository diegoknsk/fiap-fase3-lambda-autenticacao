using System.Text.Json.Serialization;

namespace FiapFastFoodAutenticacao.Core.Models;

public class AdminLoginRequest
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
    
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}

