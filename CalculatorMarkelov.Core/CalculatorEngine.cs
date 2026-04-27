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

    public double Calculate(string expression) => EvaluateExpression(expression);

    public double EvaluateExpression(string expression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expression);

        return ExpressionParser.ParseAndEvaluate(expression, this);
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

    public bool TryCalculate(string expression, out double result, out string? errorMessage) =>
        TryEvaluateExpression(expression, out result, out errorMessage);

    public bool TryEvaluateExpression(string expression, out double result, out string? errorMessage)
    {
        try
        {
            result = EvaluateExpression(expression);
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

    private sealed class ExpressionParser
    {
        private readonly string _expression;
        private readonly CalculatorEngine _calculator;
        private int _position;

        private ExpressionParser(string expression, CalculatorEngine calculator)
        {
            _expression = expression;
            _calculator = calculator;
        }

        public static double ParseAndEvaluate(string expression, CalculatorEngine calculator)
        {
            var parser = new ExpressionParser(expression, calculator);
            var result = parser.ParseExpression();

            parser.SkipWhiteSpace();

            if (!parser.IsAtEnd)
            {
                throw parser.CreateException(
                    $"Unexpected symbol '{parser.Current}' at position {parser._position + 1}.");
            }

            return result;
        }

        private bool IsAtEnd => _position >= _expression.Length;

        private char Current => _expression[_position];

        private double ParseExpression() => ParseAdditive();

        private double ParseAdditive()
        {
            var value = ParseMultiplicative();

            while (true)
            {
                SkipWhiteSpace();

                if (Match('+'))
                {
                    value = _calculator.Add(value, ParseMultiplicative());
                    continue;
                }

                if (Match('-'))
                {
                    value = _calculator.Subtract(value, ParseMultiplicative());
                    continue;
                }

                return value;
            }
        }

        private double ParseMultiplicative()
        {
            var value = ParseUnary();

            while (true)
            {
                SkipWhiteSpace();

                if (Match('*'))
                {
                    value = _calculator.Multiply(value, ParseUnary());
                    continue;
                }

                if (Match('/'))
                {
                    value = _calculator.Divide(value, ParseUnary());
                    continue;
                }

                return value;
            }
        }

        private double ParseUnary()
        {
            SkipWhiteSpace();

            if (Match('+'))
            {
                return ParseUnary();
            }

            if (Match('-'))
            {
                return -ParseUnary();
            }

            return ParsePower();
        }

        private double ParsePower()
        {
            var value = ParsePrimary();

            SkipWhiteSpace();

            if (Match('^'))
            {
                var exponent = ParseUnary();
                return _calculator.Power(value, exponent);
            }

            return value;
        }

        private double ParsePrimary()
        {
            SkipWhiteSpace();

            if (Match('('))
            {
                var value = ParseExpression();
                SkipWhiteSpace();

                if (!Match(')'))
                {
                    throw CreateException($"Missing closing parenthesis ')' at position {_position + 1}.");
                }

                return value;
            }

            if (IsAtEnd)
            {
                throw CreateException("Unexpected end of expression.");
            }

            if (char.IsDigit(Current) || Current is '.' or ',')
            {
                return ParseNumber();
            }

            throw CreateException($"Unexpected symbol '{Current}' at position {_position + 1}.");
        }

        private double ParseNumber()
        {
            var start = _position;
            var hasDigits = false;

            while (!IsAtEnd && char.IsDigit(Current))
            {
                hasDigits = true;
                _position++;
            }

            if (!IsAtEnd && Current is '.' or ',')
            {
                _position++;

                while (!IsAtEnd && char.IsDigit(Current))
                {
                    hasDigits = true;
                    _position++;
                }
            }

            if (!IsAtEnd && Current is 'e' or 'E')
            {
                var exponentPosition = _position;
                var exponentHasDigits = false;

                _position++;

                if (!IsAtEnd && Current is '+' or '-')
                {
                    _position++;
                }

                while (!IsAtEnd && char.IsDigit(Current))
                {
                    exponentHasDigits = true;
                    _position++;
                }

                if (!exponentHasDigits)
                {
                    throw CreateException(
                        $"Invalid exponent format near position {exponentPosition + 1}.");
                }
            }

            if (!hasDigits)
            {
                throw CreateException($"Expected number at position {start + 1}.");
            }

            var token = _expression[start.._position];

            try
            {
                return NumberParser.Parse(token);
            }
            catch (FormatException ex)
            {
                throw new InvalidExpressionException(ex.Message);
            }
        }

        private bool Match(char expected)
        {
            SkipWhiteSpace();

            if (IsAtEnd || Current != expected)
            {
                return false;
            }

            _position++;
            return true;
        }

        private void SkipWhiteSpace()
        {
            while (!IsAtEnd && char.IsWhiteSpace(Current))
            {
                _position++;
            }
        }

        private InvalidExpressionException CreateException(string message) => new(message);
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

public sealed class InvalidExpressionException : FormatException
{
    public InvalidExpressionException(string message)
        : base(message)
    {
    }
}
