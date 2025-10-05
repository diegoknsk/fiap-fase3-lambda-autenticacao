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
    }

    public async Task<AdminLoginResponse> AutenticarAsync(AdminLoginRequest request)
    {
        try
        {
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

            var resp = await _cognito.AdminInitiateAuthAsync(authReq);

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
        catch (NotAuthorizedException)
        {
            return new AdminLoginResponse 
            { 
                Success = false, 
                AccessToken = "", 
                Message = "Usuário ou senha inválidos" 
            };
        }
        catch (Exception)
        {
            return new AdminLoginResponse 
            { 
                Success = false, 
                AccessToken = "", 
                Message = "Erro interno" 
            };
        }
    }
}

