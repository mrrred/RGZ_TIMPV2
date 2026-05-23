using RGZ_TIMP.Models;

namespace RGZ_TIMP.Services;

/// <summary>
/// Интерфейс для сервиса сериализации и десериализации графа.
/// </summary>
public interface IGraphSerializationService
{
    /// <summary>
    /// Сохраняет проект графа в файл.
    /// </summary>
    /// <param name="path">Путь к файлу сохранения.</param>
    /// <param name="project">Модель проекта.</param>
    void Save(string path, GraphProjectModel project);

    /// <summary>
    /// Загружает проект графа из файла.
    /// </summary>
    /// <param name="path">Путь к файлу.</param>
    /// <returns>Сборка загруженного проекта.</returns>
    GraphProjectModel Load(string path);
}