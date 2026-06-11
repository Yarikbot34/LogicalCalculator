using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;


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
        Console.WriteLine(equation);
    }
}