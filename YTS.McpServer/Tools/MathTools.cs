using ModelContextProtocol.Server;
using System.ComponentModel;

namespace YTS.McpServer.Tools;

[McpServerToolType]
public sealed class MathTools
{
    [McpServerTool, Description("Calculate the square root of a number.")]
    public static string SquareRoot(double number)
    {
        if (number < 0)
        {
            return "Error: Cannot compute square root of a negative number.";
        }
        return Math.Sqrt(number).ToString("F2");
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
        if (double.IsNaN(result) || double.IsInfinity(result))
        {
            return "Error: Result is undefined or too large.";
        }
        return result.ToString("F2");
    }
}