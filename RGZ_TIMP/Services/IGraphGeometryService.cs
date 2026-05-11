using System.Windows;
using System.Windows.Media;

namespace RGZ_TIMP.Services;

public interface IGraphGeometryService
{
    Point GetEdgeStart(Point from, Point to, double radius, double parallelOffset);
    Point GetEdgeEnd(Point from, Point to, double radius, double parallelOffset);
    Point[] BuildArrow(Point tip, Vector direction);
    double CalculateParallelOffset(int edgeIndex, int totalEdges);
}