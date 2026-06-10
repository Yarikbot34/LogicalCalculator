using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public MainWindow()
    {
        DataContext = this;
        InitializeComponent();
    }
    private void SymbolButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Content is string symbol)
        {
            // Добавляем символ в конец текущего текста
            EquationTextBox.Text += symbol;
            
            // Перемещаем курсор в конец текста для удобства дальнейшего ввода
            EquationTextBox.CaretIndex = EquationTextBox.Text?.Length ?? 0;
            
            // Возвращаем фокус на текстовое поле
            EquationTextBox.Focus();
        }
    }

    private void CalculateButton_Click(object? sender, RoutedEventArgs e)
    {
        Console.Write("Решать мы это конечно не будем" + EquationTextBox.Text);
    }
}