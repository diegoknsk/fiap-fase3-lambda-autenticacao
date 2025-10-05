using Amazon.Lambda.Core;
using FiapFastFoodAutenticacao.Core.Models;
using FiapFastFoodAutenticacao.Core.Repositories;
using FiapFastFoodAutenticacao.Core.UseCases;

namespace FiapFastFoodAutenticacao.Handlers;

public class TotemHandler
{
    private readonly IAutenticacaoTotemUseCase _autenticacaoTotemUseCase;

    public TotemHandler()
    {
        var usuarioRepository = new UsuarioRepositoryMock();
        _autenticacaoTotemUseCase = new AutenticacaoTotemUseCase(usuarioRepository);
    }

    public async Task<TotemLoginResponse> HandleAsync(TotemLoginRequest request, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation($"Iniciando autenticação totem para CPF: {request.Cpf}");
            
            // Lê o secret (apenas para log, não falha se não conseguir)
            await LogSecretInfo(context);
            
            var response = await _autenticacaoTotemUseCase.AutenticarAsync(request);
            
            context.Logger.LogInformation($"Autenticação totem concluída. Success: {response.Success}");
            
            return response;
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Erro na autenticação totem: {ex.Message}");
            
            return new TotemLoginResponse
            {
                Success = false,
                Token = string.Empty,
                Message = "Erro interno do servidor",
                Usuario = null
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

