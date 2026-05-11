using RGZ_TIMP.Models;
using System.IO;
using System.Xml.Serialization;

namespace RGZ_TIMP.Services;

public sealed class GraphSerializationService : IGraphSerializationService
{
    public void Save(string path, GraphProjectModel project)
    {
        var serializer = new XmlSerializer(typeof(GraphProjectModel));
        using var stream = File.Create(path);
        serializer.Serialize(stream, project);
    }

    public GraphProjectModel Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException(path);

        var serializer = new XmlSerializer(typeof(GraphProjectModel));
        using var stream = File.OpenRead(path);
        return (GraphProjectModel)serializer.Deserialize(stream)!;
    }
}