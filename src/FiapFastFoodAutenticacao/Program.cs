using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;

namespace FiapFastFoodAutenticacao;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Este arquivo é usado para testes locais
        // Em produção, o LambdaEntryPoint é chamado pelo AWS Lambda runtime
        
        Console.WriteLine("🚀 FiapFastFoodAutenticacao Lambda - Program.cs");
        Console.WriteLine("📖 Para testes locais com Swagger, use o DebugApi:");
        Console.WriteLine("   cd src/FiapFastFoodAutenticacao.DebugApi && dotnet run");
        Console.WriteLine("   Acesse: http://localhost:5000");
        Console.WriteLine("");
        Console.WriteLine("🔐 Credenciais de teste:");
        Console.WriteLine("   Admin: admin@fiap.com / fiap@2025");
        Console.WriteLine("   Totem: 12345678901");
    }
}

