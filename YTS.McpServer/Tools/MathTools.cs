using ModelContextProtocol.Server;
using System.ComponentModel;

namespace YTS.McpServer.Tools;

[McpServerToolType]
public sealed class MathTools
{
    [McpServerTool, Description("Calculate the square root of a number.")]
    public static double SquareRoot(double number)
    {
        if (number < 0) throw new ArgumentException("Cannot compute square root of a negative number.");
        return Math.Sqrt(number);
    }

    [McpServerTool, Description("Calculate the factorial of a non-negative integer.")]
    public static long Factorial(int n)
    {
        if (n < 0) throw new ArgumentException("Factorial is not defined for negative numbers.");
        if (n > 20) throw new ArgumentException("Input too large; max is 20 to avoid overflow.");
        long result = 1;
        for (int i = 2; i <= n; i++) result *= i;
        return result;
    }

    [McpServerTool, Description("Raise a number to a power.")]
    public static double Power(double baseNum, double exponent)
    {
        return Math.Pow(baseNum, exponent);
    }
}
