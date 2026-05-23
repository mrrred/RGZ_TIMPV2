namespace RGZ_TIMP.Services;

/// <summary>
/// Интерфейс для сервиса диалоговых окон.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Показывает диалог редактирования кода узла.
    /// </summary>
    /// <param name="currentCode">Текущий код узла.</param>
    /// <returns>Новый код или null, если изменения отменены.</returns>
    string? ShowNodeCodeDialog(string currentCode);

    /// <summary>
    /// Показывает диалог редактирования ребра.
    /// </summary>
    /// <param name="predicate">Текущий предикат.</param>
    /// <param name="delay">Текущая задержка (сек).</param>
    /// <returns>Кортеж с новыми значениями предикат/задержка или null, если изменения отменены.</returns>
    (int predicate, int delay)? ShowEdgeDialog(int predicate, int delay);

    /// <summary>
    /// Показывает диалоговое окно настроек анимации.
    /// </summary>
    /// <param name="currentDurationSec">Текущая длительность анимации (сек).</param>
    /// <returns>Новая длительность или null, если отменено.</returns>
    int? ShowAnimationSettingsDialog(int currentDurationSec);

    /// <summary>
    /// Показывает диалог сохранения файла проекта.
    /// </summary>
    /// <param name="defaultName">Имя файла по умолчанию.</param>
    /// <returns>Путь для сохранения файла или null.</returns>
    string? ShowSaveFileDialog(string defaultName);

    /// <summary>
    /// Показывает диалог открытия файла.
    /// </summary>
    /// <returns>Путь к выбранному файлу или null.</returns>
    string? ShowOpenFileDialog();

    /// <summary>
    /// Показывает окно помощи для пользователя.
    /// </summary>
    void ShowHelpDialog();
}