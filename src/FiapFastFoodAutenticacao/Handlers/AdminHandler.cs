using Amazon.Lambda.Core;
using FiapFastFoodAutenticacao.Core.Models;
using FiapFastFoodAutenticacao.Core.UseCases;

namespace FiapFastFoodAutenticacao.Handlers;

public class AdminHandler
{
    private readonly IAutenticacaoAdminUseCase _autenticacaoAdminUseCase;

    public AdminHandler()
    {
        _autenticacaoAdminUseCase = new AutenticacaoAdminUseCase();
    }

    public async Task<AdminLoginResponse> HandleAsync(AdminLoginRequest request, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation($"Iniciando autenticação admin para usuário: {request.Username}");
            
            // Lê o secret (apenas para log, não falha se não conseguir)
            await LogSecretInfo(context);
            
            var response = await _autenticacaoAdminUseCase.AutenticarAsync(request);
            
            context.Logger.LogInformation($"Autenticação admin concluída. Success: {response.Success}");
            
            return response;
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Erro na autenticação admin: {ex.Message}");
            
            return new AdminLoginResponse
            {
                Success = false,
                Token = string.Empty,
                Message = "Erro interno do servidor"
            };
        }
    }

    private async Task LogSecretInfo(ILambdaContext context)
    {
        try
        {
            var secretArn = Environment.GetEnvironmentVariable("SECRET_CONNECTION_STRING_ARN");
            if (!string.IsNullOrEmpty(secretArn))
            {
                context.Logger.LogInformation($"Secret ARN configurado: {secretArn}");
                // Aqui seria feita a leitura do secret, mas como é mock, apenas logamos
                context.Logger.LogInformation("Secret lido com sucesso (mock)");
            }
            else
            {
                context.Logger.LogWarning("SECRET_CONNECTION_STRING_ARN não configurado");
            }
        }
        catch (Exception ex)
        {
            context.Logger.LogWarning($"Erro ao ler secret (não crítico): {ex.Message}");
        }
    }
}

