using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using FiapFastFoodAutenticacao.Dtos;
using FiapFastFoodAutenticacao.Handlers;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FiapFastFoodAutenticacao.CustomerLambda;

public class Function
{
    private readonly CustomerHandler _customer = new();

    public Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest req, ILambdaContext ctx)
        => FunctionHandlerAsync(req, ctx);

    public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest req, ILambdaContext ctx)
    {
        try
        {
            var path = (req.Path ?? string.Empty).ToLowerInvariant();
            var body = string.IsNullOrWhiteSpace(req.Body) ? "{}" : req.Body;
            
            // Remove o stage do path (ex: /dev/customer/identify -> /customer/identify)
            if (path.StartsWith("/dev/"))
                path = path.Substring(4);
            
            ctx.Logger.LogInformation($"Customer Lambda - Path: '{path}', Body: '{body}'");

            switch (path)
            {
                case "/customer/identify":
                    var ident = JsonSerializer.Deserialize<CustomerIdentifyModel>(body)!;
                    var identRes = await _customer.HandleIdentifyAsync(ident, ctx);
                    return Ok(identRes);

                case "/customer/register":
                    var register = JsonSerializer.Deserialize<CustomerRegisterModel>(body)!;
                    var registerRes = await _customer.HandleRegisterAsync(register, ctx);
                    return Ok(registerRes);

                case "/customer/anonymous":
                    var anonymousRes = await _customer.HandleRegisterAnonymousAsync(ctx);
                    return Ok(anonymousRes);

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
            ctx.Logger.LogError(ex.ToString());
            return new APIGatewayProxyResponse { StatusCode = 500, Body = "Internal Server Error" };
        }
    }

    private static APIGatewayProxyResponse Ok(object obj) => new()
    {
        StatusCode = 200,
        Body = JsonSerializer.Serialize(obj),
        Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" }
    };
}
