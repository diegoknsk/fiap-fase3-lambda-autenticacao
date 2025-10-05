using Amazon.Lambda.Core;

namespace FiapFastFoodAutenticacao.Tests;

public class MockLambdaContext : ILambdaContext
{
    public string AwsRequestId { get; set; } = "test-request-id";
    public IClientContext ClientContext { get; set; } = null!;
    public string FunctionName { get; set; } = "test-function";
    public string FunctionVersion { get; set; } = "1";
    public ICognitoIdentity Identity { get; set; } = null!;
    public string InvokedFunctionArn { get; set; } = "arn:aws:lambda:us-east-1:123456789012:function:test-function";
    public ILambdaLogger Logger { get; set; } = new MockLambdaLogger();
    public string LogGroupName { get; set; } = "/aws/lambda/test-function";
    public string LogStreamName { get; set; } = "2024/01/01/[$LATEST]test-stream";
    public int MemoryLimitInMB { get; set; } = 256;
    public TimeSpan RemainingTime { get; set; } = TimeSpan.FromMinutes(5);
}

public class MockLambdaLogger : ILambdaLogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[LOG] {message}");
    }

    public void LogLine(string message)
    {
        Console.WriteLine($"[LOG] {message}");
    }
}

