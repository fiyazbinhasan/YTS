using ModelContextProtocol.Server;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithTools<MathTools>();

var app = builder.Build();

app.MapMcp();

app.Run();

[McpServerToolType]
public sealed class MathTools
{
    [McpServerTool, Description("Calculate the square root of a number.")]
    public static string SquareRoot(double number)
    {
        return number < 0 
            ? "Error: Cannot compute square root of a negative number." 
            : Math.Sqrt(number).ToString("F2");
    }

    [McpServerTool, Description("Calculate the factorial of a non-negative integer.")]
    public static string Factorial(int n)
    {
        if (n < 0)
        {
            return "Error: Factorial is not defined for negative numbers.";
        }
        if (n > 20)
        {
            return "Error: Input too large; maximum supported value is 20 to avoid overflow.";
        }

        long result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result.ToString();
    }

    [McpServerTool, Description("Raise a number to a power.")]
    public static string Power(double baseNum, double exponent)
    {
        double result = Math.Pow(baseNum, exponent);
        return double.IsNaN(result) || double.IsInfinity(result) 
            ? "Error: Result is undefined or too large." 
            : result.ToString("F2");
    }
}