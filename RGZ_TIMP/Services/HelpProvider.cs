using System.Diagnostics;
using System.IO;
using System.Windows;

namespace RGZ_TIMP.Services;

/// <summary>
/// Провайдер для отображения справочной информации.
/// </summary>
public class HelpProvider
{
    private readonly IDialogService _dialogService;

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

    private static HelpProvider? _instance;

    /// <summary>
    /// Инициализирует новый экземпляр класса HelpProvider.
    /// </summary>
    /// <param name="dialogService">Сервис диалоговых окон.</param>
    public HelpProvider(IDialogService dialogService)
    {
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
    }

    /// <summary>
    /// Устанавливает глобальный экземпляр HelpProvider.
    /// </summary>
    public static void SetInstance(HelpProvider instance)
    {
        _instance = instance;
    }

    /// <summary>
    /// Показывает окно справки через глобальный экземпляр.
    /// </summary>
    /// <param name="keyword">Ключевое слово для поиска нужного раздела (необязательно).</param>
    public static void ShowHelp(string? keyword = null)
    {
        if (_instance != null)
        {
            _instance.ShowHelpInternal(keyword);
        }
        else
        {
            // Fallback - создаем временный экземпляр с DialogService по умолчанию
            var tempService = new DialogService();
            var tempProvider = new HelpProvider(tempService);
            tempProvider.ShowHelpInternal(keyword);
        }
    }

    /// <summary>
    /// Показывает окно справки.
    /// </summary>
    /// <param name="keyword">Ключевое слово для поиска нужного раздела (необязательно).</param>
    private void ShowHelpInternal(string? keyword = null)
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
                _dialogService.ShowError($"Ошибка при открытии файла справки: {ex.Message}", "Ошибка");
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
                _dialogService.ShowInfo("Файл справки не найден. Скомпилируйте проект CHM или проверьте папку help.", "Справка");
            }
        }
    }
}