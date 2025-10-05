using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using FiapFastFoodAutenticacao.Core.Models;

namespace FiapFastFoodAutenticacao.Core.UseCases;

public class AutenticacaoAdminUseCase : IAutenticacaoAdminUseCase
{
    private readonly IAmazonCognitoIdentityProvider _cognito;
    private readonly string _region;
    private readonly string _userPoolId;
    private readonly string _clientId;

    public AutenticacaoAdminUseCase()
    {
        _region     = Environment.GetEnvironmentVariable("COGNITO__REGION")     ?? "us-east-1";
        _userPoolId = Environment.GetEnvironmentVariable("COGNITO__USERPOOLID") ?? throw new InvalidOperationException("COGNITO__USERPOOLID not set");
        _clientId   = Environment.GetEnvironmentVariable("COGNITO__CLIENTID")   ?? throw new InvalidOperationException("COGNITO__CLIENTID not set");

        _cognito = new AmazonCognitoIdentityProviderClient(RegionEndpoint.GetBySystemName(_region));
        
        // Log para debug
        Console.WriteLine($"Cognito config - Region: {_region}, UserPoolId: {_userPoolId}, ClientId: {_clientId}");
        Console.WriteLine("Lambda updated with new Cognito configuration");
    }

    public async Task<AdminLoginResponse> AutenticarAsync(AdminLoginRequest request)
    {
        try
        {
            Console.WriteLine($"Tentando autenticar usuário: {request.Username}");
            
            var authReq = new AdminInitiateAuthRequest
            {
                UserPoolId = _userPoolId,
                ClientId   = _clientId,
                AuthFlow   = AuthFlowType.ADMIN_USER_PASSWORD_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", request.Username },
                    { "PASSWORD", request.Password }
                }
            };

            Console.WriteLine($"Enviando requisição para Cognito - UserPoolId: {_userPoolId}, ClientId: {_clientId}");
            var resp = await _cognito.AdminInitiateAuthAsync(authReq);
            Console.WriteLine($"Resposta do Cognito recebida - Success: {resp.AuthenticationResult != null}");

            var ok = resp.AuthenticationResult != null && !string.IsNullOrEmpty(resp.AuthenticationResult.AccessToken);
            if (!ok)
                return new AdminLoginResponse 
                { 
                    Success = false, 
                    AccessToken = "", 
                    Message = "Falha na autenticação" 
                };

            return new AdminLoginResponse
            {
                Success = true,
                TokenType = "Bearer",
                AccessToken = resp.AuthenticationResult.AccessToken,
                IdToken = resp.AuthenticationResult.IdToken,
                ExpiresIn = resp.AuthenticationResult.ExpiresIn,
                Message = "ok"
            };
        }
        catch (NotAuthorizedException ex)
        {
            Console.WriteLine($"NotAuthorizedException: {ex.Message}");
            return new AdminLoginResponse 
            { 
                Success = false, 
                AccessToken = "", 
                Message = "Usuário ou senha inválidos" 
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return new AdminLoginResponse 
            { 
                Success = false, 
                AccessToken = "", 
                Message = "Erro interno" 
            };
        }
    }
}

