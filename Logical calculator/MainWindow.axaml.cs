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
        string input = EquationTextBox.Text.Replace(" ", "");
        string equation = string.Concat(input.Select(c => Symbols.TryGetValue(c, out var r)? r:c.ToString()));
        var expression = new Expression(equation);
        Console.WriteLine("x y z    F");
        calculateEq(new[] {'x', 'y', 'z'}, 0, expression);
        
    }

    public void calculateEq(char[] parametrs, int depth, Expression exp)
    {
        if (parametrs.Length == depth)
        {
            string answ = $"{exp.Parameters["x"]} {exp.Parameters["y"]} {exp.Parameters["z"]}   {(bool)exp.Evaluate()}";
            answ = answ.Replace("True", "1").Replace("False", "0");
            Console.WriteLine(answ);
        }
        else
        {
            foreach (bool status in new[] {false, true})
            {
                exp.Parameters[parametrs[depth].ToString()] = status;
                calculateEq(parametrs, depth + 1, exp);
            }
        }
        
    }
}