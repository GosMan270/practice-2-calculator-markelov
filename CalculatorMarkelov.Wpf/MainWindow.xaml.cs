using System.Globalization;
using System.Windows;
using CalculatorMarkelov.Core;

namespace CalculatorMarkelov.Wpf;

public partial class MainWindow : Window
{
    private readonly CalculatorEngine _calculator = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        txtExpression.Focus();
        txtExpression.SelectAll();
        txtStatus.Text = "Готов к работе. Введите арифметическое выражение целиком.";
    }

    private void BtnCalculate_Click(object sender, RoutedEventArgs e)
    {
        var expression = txtExpression.Text;

        if (_calculator.TryEvaluateExpression(
                expression,
                out var result,
                out var errorMessage))
        {
            txtResult.Text = FormatResult(result);
            txtStatus.Text = BuildSuccessMessage(expression, result);
            txtStatus.Foreground = System.Windows.Media.Brushes.DarkGreen;
            return;
        }

        txtResult.Clear();
        txtStatus.Text = errorMessage ?? "Произошла неизвестная ошибка при вычислении выражения.";
        txtStatus.Foreground = System.Windows.Media.Brushes.Firebrick;
    }

    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        txtExpression.Clear();
        txtResult.Clear();
        txtStatus.Text = "Выражение очищено. Можно вводить новое выражение.";
        txtStatus.Foreground = System.Windows.Media.Brushes.SlateGray;
        txtExpression.Focus();
    }

    private static string FormatResult(double value)
    {
        if (double.IsNaN(value))
        {
            return "NaN";
        }

        if (double.IsPositiveInfinity(value))
        {
            return "+∞";
        }

        if (double.IsNegativeInfinity(value))
        {
            return "-∞";
        }

        return value.ToString("G15", CultureInfo.InvariantCulture);
    }

    private static string BuildSuccessMessage(string expression, double result)
    {
        return $"Выражение \"{expression}\" вычислено успешно. Результат: {FormatResult(result)}";
    }
}
