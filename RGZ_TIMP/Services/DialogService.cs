using System.Windows;
using System.Windows.Controls;

namespace RGZ_TIMP.Services;

/// <summary>
/// Сервис для отображения диалоговых окон пользователю.
/// </summary>
public sealed class DialogService : IDialogService
{
    /// <summary>
    /// Показывает диалог редактирования кода узла.
    /// </summary>
    /// <param name="currentCode">Текущий код узла.</param>
    /// <returns>Новый код или null, если изменения отменены.</returns>
    public string? ShowNodeCodeDialog(string currentCode)
    {
        var dialog = new Window
        {
            Title = "Код узла",
            Width = 500,
            Height = 420,
            ResizeMode = ResizeMode.NoResize,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow
        };

        var textBox = new TextBox
        {
            Margin = new Thickness(0, 0, 0, 12),
            Text = currentCode,
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Height = 230,
            MinLines = 12
        };

        var okButton = new Button
        {
            Content = "OK",
            Width = 90,
            IsDefault = true,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        okButton.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text.Trim() != "return Random.Next(1, n + 1);")
            {
                MessageBox.Show("Некорректный код узла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dialog.DialogResult = true;
        };

        var panel = new StackPanel { Margin = new Thickness(12) };
        panel.Children.Add(new TextBlock { Text = "Введите код узла:", Margin = new Thickness(0, 0, 0, 6) });
        panel.Children.Add(textBox);
        panel.Children.Add(okButton);

        dialog.Content = panel;

        if (dialog.ShowDialog() == true)
        {
            return textBox.Text;
        }

        return null;
    }

    /// <summary>
    /// Показывает диалог редактирования ребра.
    /// </summary>
    /// <param name="predicate">Текущий предикат.</param>
    /// <param name="delay">Текущая задержка (сек).</param>
    /// <returns>Кортеж с новыми значениями предикат/задержка или null, если изменения отменены.</returns>
    public (int predicate, int delay)? ShowEdgeDialog(int predicate, int delay)
    {
        var dialog = new Window
        {
            Title = "Параметры дуги",
            Width = 360,
            Height = 230,
            ResizeMode = ResizeMode.NoResize,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow
        };

        var predicateBox = new TextBox { Text = predicate.ToString(), Margin = new Thickness(0, 0, 0, 10) };
        var delayBox = new TextBox { Text = delay.ToString(), Margin = new Thickness(0, 0, 0, 12) };

        var okButton = new Button 
        { 
            Content = "OK", 
            Width = 90, 
            IsDefault = true, 
            HorizontalAlignment = HorizontalAlignment.Right 
        };

        okButton.Click += (_, _) =>
        {
            if (!int.TryParse(predicateBox.Text, out _))
            {
                MessageBox.Show("Некорректный предикат. Ожидается целое число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(delayBox.Text, out _) || int.Parse(delayBox.Text) < 0)
            {
                MessageBox.Show("Некорректная задержка. Ожидается неотрицательное целое число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dialog.DialogResult = true;
        };

        var panel = new StackPanel { Margin = new Thickness(12) };
        panel.Children.Add(new TextBlock { Text = "Предикат:", Margin = new Thickness(0, 0, 0, 4) });
        panel.Children.Add(predicateBox);
        panel.Children.Add(new TextBlock { Text = "Задержка (сек):", Margin = new Thickness(0, 0, 0, 4) });
        panel.Children.Add(delayBox);
        panel.Children.Add(okButton);

        dialog.Content = panel;

        if (dialog.ShowDialog() == true)
        {
            return (int.Parse(predicateBox.Text), int.Parse(delayBox.Text));
        }

        return null;
    }

    /// <summary>
    /// Показывает окно настроек анимации.
    /// </summary>
    /// <param name="currentDurationSec">Текущая длительность.</param>
    /// <returns>Новая длительность.</returns>
    public int? ShowAnimationSettingsDialog(int currentDurationSec)
    {
        var dialog = new Window
        {
            Title = "Настройки анимации",
            Width = 360,
            Height = 160,
            ResizeMode = ResizeMode.NoResize,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Owner = Application.Current.MainWindow
        };

        var durationBox = new TextBox { Text = currentDurationSec.ToString(), Margin = new Thickness(0, 0, 0, 10) };

        var okButton = new Button 
        { 
            Content = "OK", 
            Width = 90, 
            IsDefault = true, 
            HorizontalAlignment = HorizontalAlignment.Right 
        };

        okButton.Click += (_, _) =>
        {
            if (!int.TryParse(durationBox.Text, out int dur) || dur <= 0)
            {
                MessageBox.Show("Некорректная длительность. Ожидается положительное целое число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            dialog.DialogResult = true;
        };

        var panel = new StackPanel { Margin = new Thickness(12) };
        panel.Children.Add(new TextBlock { Text = "Длительность (сек):", Margin = new Thickness(0, 0, 0, 4) });
        panel.Children.Add(durationBox);
        panel.Children.Add(okButton);

        dialog.Content = panel;

        if (dialog.ShowDialog() == true)
        {
            return int.Parse(durationBox.Text);
        }

        return null;
    }

    /// <summary>
    /// Показывает диалог сохранения файла.
    /// </summary>
    /// <param name="defaultName">Имя по умолчанию.</param>
    /// <returns>Путь к файлу.</returns>
    public string? ShowSaveFileDialog(string defaultName)
    {
        var dlg = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
            FileName = defaultName
        };

        if (dlg.ShowDialog(Application.Current.MainWindow) == true)
        {
            return dlg.FileName;
        }

        return null;
    }

    /// <summary>
    /// Показывает диалог открытия файла.
    /// </summary>
    /// <returns>Путь к файлу.</returns>
    public string? ShowOpenFileDialog()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"
        };

        if (dlg.ShowDialog(Application.Current.MainWindow) == true)
        {
            return dlg.FileName;
        }

        return null;
    }

    /// <summary>
    /// Показывает справку программы.
    /// </summary>
    public void ShowHelpDialog()
    {
        HelpProvider.ShowHelp();
    }
}