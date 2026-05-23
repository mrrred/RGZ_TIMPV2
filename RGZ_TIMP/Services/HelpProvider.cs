using System.Diagnostics;
using System.IO;
using System.Windows;

namespace RGZ_TIMP.Services;

/// <summary>
/// Провайдер для отображения справочной информации.
/// </summary>
public static class HelpProvider
{
    /// <summary>
    /// Свойство зависимости для ключевого слова справки.
    /// </summary>
    public static readonly DependencyProperty HelpKeywordProperty =
        DependencyProperty.RegisterAttached(
            "HelpKeyword",
            typeof(string),
            typeof(HelpProvider),
            new PropertyMetadata(null));

    /// <summary>
    /// Получает ключевое слово справки для элемента.
    /// </summary>
    /// <param name="obj">Элемент зависимости.</param>
    /// <returns>Ключевое слово.</returns>
    public static string GetHelpKeyword(DependencyObject obj)
    {
        return (string)obj.GetValue(HelpKeywordProperty);
    }

    /// <summary>
    /// Устанавливает ключевое слово справки для элемента.
    /// </summary>
    /// <param name="obj">Элемент зависимости.</param>
    /// <param name="value">Значение ключевого слова.</param>
    public static void SetHelpKeyword(DependencyObject obj, string value)
    {
        obj.SetValue(HelpKeywordProperty, value);
    }

    /// <summary>
    /// Показывает окно справки.
    /// </summary>
    /// <param name="keyword">Ключевое слово для поиска нужного раздела (необязательно).</param>
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла справки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
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
                catch
                {
                    // Игнорируем ошибку при открытии HTML
                }
            }
            else
            {
                MessageBox.Show("Файл справки не найден. Скомпилируйте проект CHM или проверьте папку help.", "Справка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}