using RGZ_TIMP.Models;

namespace RGZ_TIMP.Services;

public interface IGraphSerializationService
{
    void Save(string path, GraphProjectModel project);
    GraphProjectModel Load(string path);
}