using System;
using System.Globalization;

namespace CalculatorMarkelov.Core;

public sealed class CalculatorEngine
{
    public double Add(double left, double right) => left + right;

    public double Subtract(double left, double right) => left - right;

    public double Multiply(double left, double right) => left * right;

    public double Divide(double left, double right)
    {
        if (right == 0)
        {
            throw new DivideByZeroException("Division by zero is not allowed.");
        }

        return left / right;
    }

    public double Power(double left, double right) => Math.Pow(left, right);

    public double Calculate(double left, double right, string operation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);

        return operation.Trim() switch
        {
            "+" => Add(left, right),
            "-" => Subtract(left, right),
            "*" => Multiply(left, right),
            "/" => Divide(left, right),
            "^" => Power(left, right),
            _ => throw new UnsupportedOperationException(operation)
        };
    }

    public double Calculate(string leftText, string rightText, string operation)
    {
        var left = NumberParser.Parse(leftText);
        var right = NumberParser.Parse(rightText);

        return Calculate(left, right, operation);
    }

    public bool TryCalculate(
        string leftText,
        string rightText,
        string operation,
        out double result,
        out string? errorMessage)
    {
        try
        {
            result = Calculate(leftText, rightText, operation);
            errorMessage = null;
            return true;
        }
        catch (Exception ex) when (
            ex is FormatException or
            OverflowException or
            ArgumentException or
            DivideByZeroException or
            UnsupportedOperationException)
        {
            result = 0;
            errorMessage = ex.Message;
            return false;
        }
    }
}

public static class NumberParser
{
    public static double Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new FormatException("The number value cannot be empty.");
        }

        var normalized = value.Trim().Replace(',', '.');

        if (!double.TryParse(
                normalized,
                NumberStyles.Float | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out var result))
        {
            throw new FormatException($"Invalid number: \"{value}\".");
        }

        return result;
    }
}

public sealed class UnsupportedOperationException : Exception
{
    public UnsupportedOperationException(string operation)
        : base($"Unsupported operation: \"{operation}\". Allowed operations: +, -, *, /, ^.")
    {
    }
}
