using Amazon.Lambda.Core;
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
        // Configuração específica para JWT do Customer
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "chave-ultra-secreta-para-o-projeto-fastfood1",
                ["JwtSettings:Issuer"] = "FastFoodAuth",
                ["JwtSettings:Audience"] = "FastFoodAPI-Customer",
                ["JwtSettings:ExpirationHours"] = "2" // 120 minutos = 2 horas
            })
            .Build();
            
        var tokenService = new TokenService(configuration);
        _identifyUseCase = new CustomerIdentifyUseCase(tokenService);
        _registerUseCase = new CustomerRegisterUseCase(tokenService);
        _registerAnonymousUseCase = new CustomerRegisterAnonymousUseCase(tokenService);
    }

    public async Task<CustomerTokenResponseModel> HandleIdentifyAsync(CustomerIdentifyModel request, ILambdaContext context)
    {
        try
        {
            context.Logger.LogInformation($"Iniciando identificação de customer para CPF: {MaskCpf(request.Cpf)}");
            await LogSecretInfo(context);
            
            // Teste de conexão com o banco
            var dbConnectionResult = await TestDatabaseConnection(context);
            context.Logger.LogInformation($"Teste de conexão com banco: {dbConnectionResult}");
            
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
            
            // Teste de conexão com o banco
            var dbConnectionResult = await TestDatabaseConnection(context);
            context.Logger.LogInformation($"Teste de conexão com banco: {dbConnectionResult}");
            
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

    private async Task<string> TestDatabaseConnection(ILambdaContext context)
    {
        try
        {
            var connectionString = Environment.GetEnvironmentVariable("RDS_CONNECTION_STRING");
            
            // Se não tiver variável de ambiente, usar string de teste
            if (string.IsNullOrEmpty(connectionString))
            {
                context.Logger.LogWarning("⚠️ RDS_CONNECTION_STRING não configurada, usando string de teste");
                connectionString = "server=fastfood-rds-mysql.cdiuseg40rpb.us-east-1.rds.amazonaws.com;port=3306;database=fastfooddb;user=admin;password=admin123;SslMode=Preferred";
            }
            
            context.Logger.LogInformation("Testando conexão com o banco de dados...");
            context.Logger.LogInformation($"Connection String: {MaskConnectionString(connectionString)}");
            
            using var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            await connection.OpenAsync();
            
            // Teste adicional: executar uma query simples
            using var command = new MySql.Data.MySqlClient.MySqlCommand("SELECT 1 as test", connection);
            var result = await command.ExecuteScalarAsync();
            
            context.Logger.LogInformation("✅ Conexão com banco estabelecida com sucesso!");
            context.Logger.LogInformation($"✅ Query de teste executada: {result}");
            return "SUCESSO - Conectado ao banco e query executada";
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"❌ Falha na conexão com banco: {ex.Message}");
            return $"FALHA - Erro: {ex.Message}";
        }
    }

    private string MaskConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "***";
            
        // Mascarar a senha na string de conexão para logs
        return connectionString.Replace("password=admin123", "password=***");
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
