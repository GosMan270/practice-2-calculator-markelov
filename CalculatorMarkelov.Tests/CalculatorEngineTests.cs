using System;
using CalculatorMarkelov.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculatorMarkelov.Tests;

[TestClass]
public class CalculatorEngineTests
{
    private CalculatorEngine _calculator = null!;

    [TestInitialize]
    public void Setup()
    {
        _calculator = new CalculatorEngine();
    }

    [DataTestMethod]
    [DataRow(2.0, 3.0, 5.0)]
    [DataRow(-5.0, 2.0, -3.0)]
    [DataRow(0.0, 0.0, 0.0)]
    public void Add_ReturnsCorrectResult(double left, double right, double expected)
    {
        var actual = _calculator.Add(left, right);

        Assert.AreEqual(expected, actual, 0.0000001);
    }

    [DataTestMethod]
    [DataRow(8.0, 3.0, 5.0)]
    [DataRow(-5.0, -2.0, -3.0)]
    [DataRow(0.0, 7.0, -7.0)]
    public void Subtract_ReturnsCorrectResult(double left, double right, double expected)
    {
        var actual = _calculator.Subtract(left, right);

        Assert.AreEqual(expected, actual, 0.0000001);
    }

    [DataTestMethod]
    [DataRow(4.0, 5.0, 20.0)]
    [DataRow(-3.0, 2.0, -6.0)]
    [DataRow(0.0, 100.0, 0.0)]
    public void Multiply_ReturnsCorrectResult(double left, double right, double expected)
    {
        var actual = _calculator.Multiply(left, right);

        Assert.AreEqual(expected, actual, 0.0000001);
    }

    [DataTestMethod]
    [DataRow(10.0, 2.0, 5.0)]
    [DataRow(7.5, 2.5, 3.0)]
    [DataRow(-12.0, 3.0, -4.0)]
    public void Divide_ReturnsCorrectResult(double left, double right, double expected)
    {
        var actual = _calculator.Divide(left, right);

        Assert.AreEqual(expected, actual, 0.0000001);
    }

    [TestMethod]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        Assert.ThrowsException<DivideByZeroException>(() => _calculator.Divide(10, 0));
    }

    [DataTestMethod]
    [DataRow(2.0, 3.0, 8.0)]
    [DataRow(9.0, 0.5, 3.0)]
    [DataRow(5.0, 0.0, 1.0)]
    public void Power_ReturnsCorrectResult(double left, double right, double expected)
    {
        var actual = _calculator.Power(left, right);

        Assert.AreEqual(expected, actual, 0.0000001);
    }

    [DataTestMethod]
    [DataRow(2.0, 3.0, "+", 5.0)]
    [DataRow(9.0, 4.0, "-", 5.0)]
    [DataRow(6.0, 7.0, "*", 42.0)]
    [DataRow(12.0, 4.0, "/", 3.0)]
    [DataRow(3.0, 4.0, "^", 81.0)]
    public void Calculate_WithNumericArguments_ReturnsCorrectResult(
        double left,
        double right,
        string operation,
        double expected)
    {
        var actual = _calculator.Calculate(left, right, operation);

        Assert.AreEqual(expected, actual, 0.0000001);
    }

    [TestMethod]
    public void Calculate_WithUnsupportedOperation_ThrowsUnsupportedOperationException()
    {
        Assert.ThrowsException<UnsupportedOperationException>(() => _calculator.Calculate(2, 3, "%"));
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void Calculate_WithEmptyOperation_ThrowsArgumentException(string? operation)
    {
        Assert.ThrowsException<ArgumentException>(() => _calculator.Calculate(2, 3, operation!));
    }

    [TestMethod]
    public void Calculate_WithNullOperation_ThrowsException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _calculator.Calculate(2, 3, (string)null!));
    }

    [DataTestMethod]
    [DataRow("2", "3", "+", 5.0)]
    [DataRow("10,5", "2", "-", 8.5)]
    [DataRow("-4", "5", "*", -20.0)]
    [DataRow("8", "2", "/", 4.0)]
    [DataRow("2", "5", "^", 32.0)]
    public void Calculate_WithStringArguments_ReturnsCorrectResult(
        string left,
        string right,
        string operation,
        double expected)
    {
        var actual = _calculator.Calculate(left, right, operation);

        Assert.AreEqual(expected, actual, 0.0000001);
    }

    [TestMethod]
    public void TryCalculate_WithValidInput_ReturnsTrueAndResult()
    {
        var success = _calculator.TryCalculate("12", "3", "/", out var result, out var errorMessage);

        Assert.IsTrue(success);
        Assert.AreEqual(4.0, result, 0.0000001);
        Assert.IsNull(errorMessage);
    }

    [TestMethod]
    public void TryCalculate_WithInvalidNumber_ReturnsFalseAndErrorMessage()
    {
        var success = _calculator.TryCalculate("abc", "3", "+", out var result, out var errorMessage);

        Assert.IsFalse(success);
        Assert.AreEqual(0.0, result, 0.0000001);
        Assert.IsFalse(string.IsNullOrWhiteSpace(errorMessage));
        StringAssert.Contains(errorMessage!, "Invalid number");
    }

    [TestMethod]
    public void TryCalculate_WithDivisionByZero_ReturnsFalseAndErrorMessage()
    {
        var success = _calculator.TryCalculate("10", "0", "/", out var result, out var errorMessage);

        Assert.IsFalse(success);
        Assert.AreEqual(0.0, result, 0.0000001);
        Assert.IsFalse(string.IsNullOrWhiteSpace(errorMessage));
        StringAssert.Contains(errorMessage!, "Division by zero");
    }

    [DataTestMethod]
    [DataRow("15", 15.0)]
    [DataRow("15.25", 15.25)]
    [DataRow("15,25", 15.25)]
    [DataRow("  -7,5  ", -7.5)]
    public void NumberParser_Parse_ValidValues_ReturnsCorrectNumber(string input, double expected)
    {
        var actual = NumberParser.Parse(input);

        Assert.AreEqual(expected, actual, 0.0000001);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void NumberParser_Parse_EmptyValues_ThrowsFormatException(string? input)
    {
        Assert.ThrowsException<FormatException>(() => NumberParser.Parse(input));
    }

    [DataTestMethod]
    [DataRow("abc")]
    [DataRow("12..5")]
    [DataRow("5,2,1")]
    public void NumberParser_Parse_InvalidValues_ThrowsFormatException(string input)
    {
        Assert.ThrowsException<FormatException>(() => NumberParser.Parse(input));
    }

    [TestMethod]
    public void UnsupportedOperationException_MessageContainsOperation()
    {
        var exception = new UnsupportedOperationException("%");

        StringAssert.Contains(exception.Message, "%");
        StringAssert.Contains(exception.Message, "+");
        StringAssert.Contains(exception.Message, "^");
    }
}
