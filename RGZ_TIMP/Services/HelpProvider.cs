using System.Diagnostics;
using System.IO;
using System.Windows;

namespace RGZ_TIMP.Services;

public static class HelpProvider
{
    public static readonly DependencyProperty HelpKeywordProperty =
        DependencyProperty.RegisterAttached(
            "HelpKeyword",
            typeof(string),
            typeof(HelpProvider),
            new PropertyMetadata(null));

    public static string GetHelpKeyword(DependencyObject obj)
    {
        return (string)obj.GetValue(HelpKeywordProperty);
    }

    public static void SetHelpKeyword(DependencyObject obj, string value)
    {
        obj.SetValue(HelpKeywordProperty, value);
    }

    public static void ShowHelp(string? keyword = null)
    {
        string chmPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "help", "Help.chm");
        if (File.Exists(chmPath))
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = chmPath,
                    UseShellExecute = true
                };
                Process.Start(processInfo);
                // System.Windows.Forms.Help.ShowHelp is better but requires WinForms reference.
                // Doing basic CHM opening via Shell Execute natively opens the CHM viewer.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла справки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            // Fallback to HTML files if CHM not compiled yet
            string htmlFile = string.IsNullOrEmpty(keyword) ? "concept.html" : keyword;
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "help", htmlFile);
            if (File.Exists(htmlPath))
            {
                try
                {
                    var processInfo = new ProcessStartInfo
                    {
                        FileName = htmlPath,
                        UseShellExecute = true
                    };
                    Process.Start(processInfo);
                }
                catch { }
            }
            else
            {
                MessageBox.Show("Файл справки не найден. Скомпилируйте проект CHM или проверьте папку help.", "Справка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}