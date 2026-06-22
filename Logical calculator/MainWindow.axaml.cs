using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Interactivity;
using Avalonia.Styling;
using NCalc;
using ClosedXML.Excel;

namespace Logical_calculator;

public partial class MainWindow : Window
{
    private Result[] results;
    private string[] parametrs;
    private string equation;
    
    public ObservableCollection<string> KeyboardSymbols { get; } = new ObservableCollection<string>
    {
        "(", ")", 
        "¬", "∧", "∨", "⊕", "⇒", "≡",
        "x", "y", "z", "n", "m","u", "0", "1"
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
        if (EquationTextBox.Text != "")
        {
            Result.countFalse = 0;
            Result.countTrue = 0;
            bool isTavtology;
            ErrorTextBlock.Text = "";
            string input = EquationTextBox.Text.Replace(" ", "");
            equation = string.Concat(input.Select(c => Symbols.TryGetValue(c, out var r)? r:c.ToString()));
            try
            {
                var expression = new Expression(equation);
                parametrs = expression.GetParameterNames().ToArray();
                int resultCount = (int)Math.Pow(2, parametrs.Length);
                results = new Result[resultCount];
                CalculateEq(parametrs, parametrs.Length, expression);
                isTavtology = (int)Math.Pow(2, parametrs.Length) == Result.countTrue || (int)Math.Pow(2, parametrs.Length) == Result.countFalse;
                PositiveCountText.Text = Result.countTrue.ToString();
                NegativeCountText.Text = Result.countFalse.ToString();
                TautologyText.Text = isTavtology ? "Да" : "Нет";
                ExportBlock.IsVisible = true;
                DrawTable();
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = ex.Message;
            }
        }
    }

    private void ExportXlsxButton_Click(object sender, RoutedEventArgs e)
    {
        if (results.Length != 0) { ExportXlsx(); ExportInfo.Text = ""; }
    }
    private void ExportMarkdownButton_Click(object sender, RoutedEventArgs e)
    {
        if (results.Length != 0) { ExportMarkDown(); ExportInfo.Text = "Таблица скопирована в буфер обмена"; }
    }
    private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
    {
        var app = Application.Current;
        if (app != null)
        {
            var currentTheme = app.ActualThemeVariant;
            app.RequestedThemeVariant = currentTheme == ThemeVariant.Dark 
                ? ThemeVariant.Light 
                : ThemeVariant.Dark;
            
            ThemeToggleButton.Content = currentTheme == ThemeVariant.Dark ? "🌙" : "☀️";
        }
    }


    public void ExportXlsx()
    {
        var topLevel = GetTopLevel(this);
        if (topLevel == null) return;
        string[] exParametrs = parametrs.Append(equation).ToArray();
        using (var wBook = new XLWorkbook())
        {
            var worksheet = wBook.Worksheets.Add(equation);
            worksheet.Cell(1, 1).InsertTable(results.Select(r => r.getData()));
            for(int i = 0; i < exParametrs.Length; i++)
            {
                worksheet.Cell(1, i+1).Value = exParametrs[i];
            }
            worksheet.Columns().AdjustToContents();
            try
            {
                var file = topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Сохранить файл Excel", SuggestedFileName = "result.xlsx", DefaultExtension = "xlsx",
                    FileTypeChoices = new[]
                    {
                        new FilePickerFileType("Excel файлы")
                            { Patterns = new[] { "*.xlsx" } },
                        new FilePickerFileType("Все файлы") { Patterns = new[] { "*.*" } }
                    }
                });
                string filePath = file.Result.Path.AbsolutePath;
                wBook.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }

    public void ExportMarkDown()
    {
        string[] exParametrs = parametrs.Append(equation).ToArray();
        string table = "| " + string.Join(" | ", exParametrs) + " |\n| ";
        for (int i = 0; i < exParametrs.Length; i++)
        {
            table += ":-: |";
        }
        table += "\n";
        foreach (var result in results)
        {
            table += "| " + string.Join(" | ", result.getData()) + " |\n";
        }
        Clipboard.SetTextAsync(table);
    }
    
    public void CalculateEq(string[] parametrs, int depth, Expression exp, int numberOfResult = 0)
    {
        if (depth == 0)
        {
            Result res = new Result(exp);
            results[numberOfResult] = res;
        }
        else
        {
            foreach (bool status in new[] {false, true})
            {
                exp.Parameters[parametrs[^depth].ToString()] = status;
                if (status) { numberOfResult += (int)Math.Pow(2, depth-1);}
                CalculateEq(parametrs, depth - 1, exp, numberOfResult);
            }
        }
        
    }
    
    
private void DrawTable()
{
    ResultGrid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
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
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            Padding = new Thickness(5),
            FontSize = 32
        };
        
        Avalonia.Media.IBrush background = Avalonia.Media.Brushes.LightGray;
        if (isHeader) background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.SlateGray);
        else if (isResult && text == "1" ) background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.DarkGreen);
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