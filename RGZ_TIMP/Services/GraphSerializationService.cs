using RGZ_TIMP.Models;
using System.IO;
using System.Xml.Serialization;

namespace RGZ_TIMP.Services;

/// <summary>
/// Сервис для сериализации и десериализации графа в XML.
/// </summary>
public sealed class GraphSerializationService : IGraphSerializationService
{
    /// <summary>
    /// Сохраняет проект в файл.
    /// </summary>
    /// <param name="path">Путь к файлу.</param>
    /// <param name="project">Модель проекта.</param>
    public void Save(string path, GraphProjectModel project)
    {
        var serializer = new XmlSerializer(typeof(GraphProjectModel));
        using var stream = File.Create(path);

        serializer.Serialize(stream, project);
    }

    /// <summary>
    /// Загружает проект из файла.
    /// </summary>
    /// <param name="path">Путь к файлу.</param>
    /// <returns>Загруженная модель проекта.</returns>
    /// <exception cref="FileNotFoundException">Выбрасывается, если файл не найден.</exception>
    public GraphProjectModel Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(path);
        }

        var serializer = new XmlSerializer(typeof(GraphProjectModel));
        using var stream = File.OpenRead(path);

        return (GraphProjectModel)serializer.Deserialize(stream)!;
    }
}