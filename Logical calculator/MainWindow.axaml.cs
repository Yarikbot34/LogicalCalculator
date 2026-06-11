using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        "x", "y", "z", "n", "m","u"
    };

    public Dictionary<char, string> Symbols { get; } = new Dictionary<char, string>
    {
        ['¬'] = "!", ['∧'] = " and ", ['∨'] = " or ", ['⊕'] = "", ['⇒'] = " <= ", ['≡'] = " == "
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
        char[] parametrs = new[] { 'x', 'y', 'z' };
        string input = EquationTextBox.Text.Replace(" ", "");
        string equation = string.Concat(input.Select(c => Symbols.TryGetValue(c, out var r)? r:c.ToString()));
        var expression = new Expression(equation);
        int resultCount = 2;
        for (int i = 1; i < parametrs.Length; i++) { resultCount *= 2;}
        Console.WriteLine(resultCount);
        results = new Result[resultCount];
        calculateEq(parametrs, parametrs.Length, expression);
        
    }

    public void calculateEq(char[] parametrs, int depth, Expression exp, int numberOfResult = 0)
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
    
    
}