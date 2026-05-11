using System.Windows;
using System.Windows.Controls;

namespace RGZ_TIMP.Services;

public sealed class DialogService : IDialogService
{
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
        var okButton = new Button { Content = "OK", Width = 90, IsDefault = true, HorizontalAlignment = HorizontalAlignment.Right };
        okButton.Click += (_, _) => dialog.DialogResult = true;
        var panel = new StackPanel { Margin = new Thickness(12) };
        panel.Children.Add(new TextBlock { Text = "Введите код узла:", Margin = new Thickness(0, 0, 0, 6) });
        panel.Children.Add(textBox);
        panel.Children.Add(okButton);
        dialog.Content = panel;
        return dialog.ShowDialog() == true ? textBox.Text : null;
    }

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
        var okButton = new Button { Content = "OK", Width = 90, IsDefault = true, HorizontalAlignment = HorizontalAlignment.Right };
        okButton.Click += (_, _) => dialog.DialogResult = true;
        var panel = new StackPanel { Margin = new Thickness(12) };
        panel.Children.Add(new TextBlock { Text = "Предикат:", Margin = new Thickness(0, 0, 0, 4) });
        panel.Children.Add(predicateBox);
        panel.Children.Add(new TextBlock { Text = "Задержка (сек):", Margin = new Thickness(0, 0, 0, 4) });
        panel.Children.Add(delayBox);
        panel.Children.Add(okButton);
        dialog.Content = panel;
        if (dialog.ShowDialog() == true &&
            int.TryParse(predicateBox.Text, out var newPred) &&
            int.TryParse(delayBox.Text, out var newDelay))
            return (newPred, newDelay);
        return null;
    }

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
        var okButton = new Button { Content = "OK", Width = 90, IsDefault = true, HorizontalAlignment = HorizontalAlignment.Right };
        okButton.Click += (_, _) => dialog.DialogResult = true;
        var panel = new StackPanel { Margin = new Thickness(12) };
        panel.Children.Add(new TextBlock { Text = "Длительность (сек):", Margin = new Thickness(0, 0, 0, 4) });
        panel.Children.Add(durationBox);
        panel.Children.Add(okButton);
        dialog.Content = panel;
        if (dialog.ShowDialog() == true && int.TryParse(durationBox.Text, out var dur))
            return dur;
        return null;
    }

    public string? ShowSaveFileDialog(string defaultName)
    {
        var dlg = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
            FileName = defaultName
        };
        return dlg.ShowDialog(Application.Current.MainWindow) == true ? dlg.FileName : null;
    }

    public string? ShowOpenFileDialog()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"
        };
        return dlg.ShowDialog(Application.Current.MainWindow) == true ? dlg.FileName : null;
    }

    public void ShowHelpDialog()
    {
        MessageBox.Show("Справка по приложению:\n- Создайте или откройте проект.\n- Используйте правую кнопку мыши, чтобы добавить узлы.\n- Для соединения узлов зажмите Ctrl и потяните мышь от черного кружка от узла к другому узлу.\n- Задайте настройки и запустите анимацию.", 
            "Справка", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}