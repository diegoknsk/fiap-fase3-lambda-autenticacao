using System.Text.Json.Serialization;

namespace FiapFastFoodAutenticacao.Dtos;

public class CustomerIdentifyModel
{
    [JsonPropertyName("cpf")]
    public string Cpf { get; set; } = string.Empty;
}

public class CustomerRegisterModel
{
    [JsonPropertyName("cpf")]
    public string Cpf { get; set; } = string.Empty;
}

public class CustomerTokenResponseModel
{
    public string Token { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public DateTime ExpiresAt { get; set; }
}
