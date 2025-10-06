using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FiapFastFoodAutenticacao.Core.Models;
using FiapFastFoodAutenticacao.Handlers;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FiapFastFoodAutenticacao.AdminLambda;

public class Function
{
    private readonly AdminHandler _admin = new();

    public Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest req, ILambdaContext ctx)
        => FunctionHandlerAsync(req, ctx);

    public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest req, ILambdaContext ctx)
    {
        try
        {
            var body = string.IsNullOrWhiteSpace(req.Body) ? "{}" : req.Body;
            var loginReq = JsonSerializer.Deserialize<AdminLoginRequest>(body)!;
            var result = await _admin.HandleAsync(loginReq, ctx);

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(result),
                Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" }
            };
        }
        catch (UnauthorizedAccessException)
        {
            return new APIGatewayProxyResponse { StatusCode = 401, Body = "Unauthorized" };
        }
        catch (Exception ex)
        {
            ctx.Logger.LogError(ex.ToString());
            return new APIGatewayProxyResponse { StatusCode = 500, Body = "Internal Server Error" };
        }
    }
}
