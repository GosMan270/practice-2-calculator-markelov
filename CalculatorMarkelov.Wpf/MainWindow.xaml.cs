using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
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
        txtFirstNumber.Focus();
        txtStatus.Text = "Готов к работе. Введите значения и выберите операцию.";
    }

    private void BtnCalculate_Click(object sender, RoutedEventArgs e)
    {
        var operation = GetSelectedOperation();

        if (_calculator.TryCalculate(
                txtFirstNumber.Text,
                txtSecondNumber.Text,
                operation,
                out var result,
                out var errorMessage))
        {
            txtResult.Text = FormatResult(result);
            txtStatus.Text = BuildSuccessMessage(operation, result);
            txtStatus.Foreground = System.Windows.Media.Brushes.DarkGreen;
            return;
        }

        txtResult.Clear();
        txtStatus.Text = errorMessage ?? "Произошла неизвестная ошибка вычисления.";
        txtStatus.Foreground = System.Windows.Media.Brushes.Firebrick;
    }

    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        txtFirstNumber.Clear();
        txtSecondNumber.Clear();
        txtResult.Clear();
        cmbOperation.SelectedIndex = 0;
        txtStatus.Text = "Поля очищены. Можно вводить новые данные.";
        txtStatus.Foreground = System.Windows.Media.Brushes.SlateGray;
        txtFirstNumber.Focus();
    }

    private string GetSelectedOperation()
    {
        if (cmbOperation.SelectedItem is ComboBoxItem item &&
            item.Content is string operation &&
            !string.IsNullOrWhiteSpace(operation))
        {
            return operation;
        }

        return "+";
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

    private static string BuildSuccessMessage(string operation, double result)
    {
        return $"Операция \"{operation}\" выполнена успешно. Результат: {FormatResult(result)}";
    }
}
