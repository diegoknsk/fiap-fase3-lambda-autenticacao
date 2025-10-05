using Amazon.Lambda.Core;
using FiapFastFoodAutenticacao.Core.Repositories;
using FiapFastFoodAutenticacao.Core.UseCases;
using FiapFastFoodAutenticacao.Dtos;
using FiapFastFoodAutenticacao.Services;
using Microsoft.Extensions.Configuration;

namespace FiapFastFoodAutenticacao.Handlers;

public class CustomerHandler
{
    private readonly CustomerIdentifyUseCase _identifyUseCase;
    private readonly CustomerRegisterUseCase _registerUseCase;
    private readonly CustomerRegisterAnonymousUseCase _registerAnonymousUseCase;

    public CustomerHandler()
    {
        var usuarioRepository = new UsuarioRepositoryMock();
        
        // Configuração simples para o TokenService
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "FiapFastFoodSuperSecretKeyForJWTTokenGeneration2025!",
                ["JwtSettings:Issuer"] = "FiapFastFood",
                ["JwtSettings:Audience"] = "FiapFastFood",
                ["JwtSettings:ExpirationHours"] = "3"
            })
            .Build();
            
        var tokenService = new TokenService(configuration);
        _identifyUseCase = new CustomerIdentifyUseCase(usuarioRepository, tokenService);
        _registerUseCase = new CustomerRegisterUseCase(usuarioRepository, tokenService);
        _registerAnonymousUseCase = new CustomerRegisterAnonymousUseCase(usuarioRepository, tokenService);
    }

    public async Task<CustomerTokenResponseModel> HandleIdentifyAsync(CustomerIdentifyModel request, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation($"Iniciando identificação de customer para CPF: {MaskCpf(request.Cpf)}");
            await LogSecretInfo(context);
            
            var response = await _identifyUseCase.ExecuteAsync(request.Cpf);
            
            context.Logger.LogInformation($"Identificação de customer concluída. CustomerId: {response.CustomerId}");
            
            return response;
        }
        catch (UnauthorizedAccessException)
        {
            context.Logger.LogWarning($"CPF não encontrado: {MaskCpf(request.Cpf)}");
            throw;
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Erro na identificação de customer: {ex.Message}");
            throw;
        }
    }

    public async Task<CustomerTokenResponseModel> HandleRegisterAsync(CustomerRegisterModel request, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation($"Iniciando registro de customer para CPF: {MaskCpf(request.Cpf)}");
            await LogSecretInfo(context);
            
            var response = await _registerUseCase.ExecuteAsync(request);
            
            context.Logger.LogInformation($"Registro de customer concluído. CustomerId: {response.CustomerId}");
            
            return response;
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Erro no registro de customer: {ex.Message}");
            throw;
        }
    }

    public async Task<CustomerTokenResponseModel> HandleRegisterAnonymousAsync(ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation("Iniciando registro de customer anônimo");
            await LogSecretInfo(context);
            
            var response = await _registerAnonymousUseCase.ExecuteAsync();
            
            context.Logger.LogInformation($"Registro de customer anônimo concluído. CustomerId: {response.CustomerId}");
            
            return response;
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Erro no registro de customer anônimo: {ex.Message}");
            throw;
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

    private string MaskCpf(string cpf)
    {
        if (string.IsNullOrEmpty(cpf) || cpf.Length < 4)
            return "***";
        
        return cpf.Substring(0, 3) + "***" + cpf.Substring(cpf.Length - 2);
    }
}
