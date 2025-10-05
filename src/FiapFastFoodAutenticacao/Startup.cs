using FiapFastFoodAutenticacao.Contracts;
using FiapFastFoodAutenticacao.Dtos;
using FiapFastFoodAutenticacao.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace FiapFastFoodAutenticacao;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { 
                Title = "FiapFastFood Lambda API", 
                Version = "v1",
                Description = "API de Autenticação - Lambda Function"
            });
        });
        
        // DI limitado - injetar IAuthService
        services.AddSingleton<IAuthService, AuthService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Swagger sempre disponível no Lambda (para debug)
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "FiapFastFood Lambda API v1");
            c.RoutePrefix = string.Empty; // Para acessar o Swagger na raiz
        });

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            // Endpoint de status
            endpoints.MapGet("/", async context =>
            {
                var message = "FiapFastFood Lambda - Autenticação\nAcesse /swagger para documentação da API";
                var bytes = Encoding.UTF8.GetBytes(message);
                await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            });

            // Endpoints básicos para o Lambda (sem Swagger por enquanto)
            endpoints.MapGet("/status", async context =>
            {
                var message = "FiapFastFood Lambda - Autenticação\nStatus: OK";
                var bytes = Encoding.UTF8.GetBytes(message);
                await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            });
        });
    }
}
