using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FiapFastFoodAutenticacao.Handlers
{
    public class Dispatcher
    {
        private readonly AdminHandler _admin = new();
        private readonly CustomerHandler _customer = new();

        // Alias "seguro" que muitos exemplos usam:
        public Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest req, ILambdaContext ctx)
            => FunctionHandlerAsync(req, ctx);

        // Seu handler principal
        public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest req, ILambdaContext ctx)
        {
            try
            {
                var path = (req.Path ?? string.Empty).ToLowerInvariant();

                switch (path)
                {
                    case "/autenticacao/admin":
                    case "/autenticacaoadmin":
                        var adminReq = JsonSerializer.Deserialize<Core.Models.AdminLoginRequest>(req.Body);
                        var adminRes = await _admin.HandleAsync(adminReq!, ctx);
                        return Ok(adminRes);

                    case "/autenticacao/identify":
                    case "/autenticacaototem/identify":
                        var identReq = JsonSerializer.Deserialize<Dtos.CustomerIdentifyModel>(req.Body);
                        var identRes = await _customer.HandleIdentifyAsync(identReq!, ctx);
                        return Ok(identRes);

                    case "/autenticacao/register":
                    case "/autenticacaototem/register":
                        var regReq = JsonSerializer.Deserialize<Dtos.CustomerRegisterModel>(req.Body);
                        var regRes = await _customer.HandleRegisterAsync(regReq!, ctx);
                        return Ok(regRes);

                    case "/autenticacao/anonymous":
                    case "/autenticacaototem/anonymous":
                        var anonRes = await _customer.HandleRegisterAnonymousAsync(ctx);
                        return Ok(anonRes);

                    default:
                        return new APIGatewayProxyResponse { StatusCode = 404, Body = "Not found" };
                }
            }
            catch (UnauthorizedAccessException)
            {
                return new APIGatewayProxyResponse { StatusCode = 401, Body = "Unauthorized" };
            }
            catch (Exception ex)
            {
                ctx.Logger.LogError($"Erro: {ex}");
                return new APIGatewayProxyResponse { StatusCode = 500, Body = "Internal Server Error" };
            }
        }

        private static APIGatewayProxyResponse Ok(object obj) =>
            new()
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(obj),
                Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" }
            };
    }
}
