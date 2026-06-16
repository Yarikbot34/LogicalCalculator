using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NCalc;


namespace Logical_calculator;

public partial class MainWindow : Window
{
    public Result[] results;
    
    public ObservableCollection<string> KeyboardSymbols { get; } = new ObservableCollection<string>
    {
        "(", ")", 
        "¬", "∧", "∨", "⊕", "⇒", "≡",
        "x", "y", "z", "n", "m","u", "a", "b"
    };

    public Dictionary<char, string> Symbols { get; } = new Dictionary<char, string>
    {
        ['¬'] = "!", ['∧'] = " and ", ['∨'] = " or ", ['⊕'] = " != ", ['⇒'] = " <= ", ['≡'] = " == "
    };
    
    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
    }
    private void SymbolButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Content is string symbol)
        {
            EquationTextBox.Text += symbol;
            
            EquationTextBox.CaretIndex = EquationTextBox.Text?.Length ?? 0;
            
            EquationTextBox.Focus();
        }
    }

    private void CalculateButton_Click(object? sender, RoutedEventArgs e)
    {
        string input = EquationTextBox.Text.Replace(" ", "");
        string equation = string.Concat(input.Select(c => Symbols.TryGetValue(c, out var r)? r:c.ToString()));
        Console.WriteLine(equation);
        var expression = new Expression(equation);
        string[] parametrs = expression.GetParameterNames().ToArray();
        int resultCount = 2;
        for (int i = 1; i < parametrs.Length; i++) { resultCount *= 2;}
        Console.WriteLine(resultCount);
        results = new Result[resultCount];
        calculateEq(parametrs, parametrs.Length, expression);
        DrawTable();
    }

    public void calculateEq(string[] parametrs, int depth, Expression exp, int numberOfResult = 0)
    {
        if (depth == 0)
        {
            Result res = new Result(exp);
            results[numberOfResult] = res;
            Console.WriteLine($"Writed result №{numberOfResult + 1}");
        }
        else
        {
            foreach (bool status in new[] {false, true})
            {
                exp.Parameters[parametrs[^depth].ToString()] = status;
                if (status) { numberOfResult += (int)Math.Pow(2, depth-1);}
                calculateEq(parametrs, depth - 1, exp, numberOfResult);
            }
        }
        
    }
    
private void DrawTable()
{
    ResultGrid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
    ResultGrid.Children.Clear();
    ResultGrid.RowDefinitions.Clear();
    ResultGrid.ColumnDefinitions.Clear();
    
    var paramNames = results[0].values.Keys.ToList();
    int colCount = paramNames.Count + 1; 
    
    for (int i = 0; i < colCount; i++)
    {
        ResultGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
    }
    
    for (int i = 0; i <= results.Length; i++)
    {
        ResultGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
    }
    
    for (int i = 0; i < paramNames.Count; i++)
    {
        var cell = CreateCell(paramNames[i], isHeader: true);
        Grid.SetRow(cell, 0);
        Grid.SetColumn(cell, i);
        ResultGrid.Children.Add(cell);
    }
    var fHeader = CreateCell("F", isHeader: true);
    Grid.SetRow(fHeader, 0);
    Grid.SetColumn(fHeader, colCount - 1);
    ResultGrid.Children.Add(fHeader);
    
    for (int r = 0; r < results.Length; r++)
    {
        for (int c = 0; c < paramNames.Count; c++)
        {
            string val = results[r].values[paramNames[c]] ? "1" : "0"; 
            var cell = CreateCell(val);
            Grid.SetRow(cell, r + 1);
            Grid.SetColumn(cell, c);
            ResultGrid.Children.Add(cell);
        }
        

        string resVal = results[r].result ? "1" : "0";
        var resCell = CreateCell(resVal, isResult: true);
        Grid.SetRow(resCell, r + 1);
        Grid.SetColumn(resCell, colCount - 1);
        ResultGrid.Children.Add(resCell);
    }
}


    private Border CreateCell(string text, bool isHeader = false, bool isResult = false)
    {
        var tb = new TextBlock
        {
            Text = text,
            Foreground = Avalonia.Media.Brushes.Black,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Padding = new Thickness(5),
            FontSize = 32
        };
        
        Avalonia.Media.IBrush background = Avalonia.Media.Brushes.LightGray;
        if (isHeader) background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.SlateGray);
        else if (isResult && text == "1") background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.DarkGreen);
        else if (isResult && text == "0") background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.DarkRed);
        
        return new Border
        {
            Child = tb,
            Background = background,
            BorderBrush = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Black),
            BorderThickness = new Thickness(2), 
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
        };
    }
}