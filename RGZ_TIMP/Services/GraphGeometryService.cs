using System.Windows;
using System.Windows.Media;

namespace RGZ_TIMP.Services;

public sealed class GraphGeometryService : IGraphGeometryService
{
    private const double ArrowSize = 10;

    public Point GetEdgeStart(Point from, Point to, double radius, double parallelOffset)
    {
        var direction = to - from;
        if (direction.Length < 0.001) return from;
        direction.Normalize();
        var normal = new Vector(-direction.Y, direction.X);
        var radial = Math.Sqrt(Math.Max(0, radius * radius - Math.Min(parallelOffset * parallelOffset, radius * radius - 1)));
        return from + direction * radial + normal * parallelOffset;
    }

    public Point GetEdgeEnd(Point from, Point to, double radius, double parallelOffset)
    {
        var direction = to - from;
        if (direction.Length < 0.001) return to;
        direction.Normalize();
        var normal = new Vector(-direction.Y, direction.X);
        var radial = Math.Sqrt(Math.Max(0, radius * radius - Math.Min(parallelOffset * parallelOffset, radius * radius - 1)));
        return to - direction * radial + normal * parallelOffset;
    }

    public Point[] BuildArrow(Point tip, Vector direction)
    {
        direction.Normalize();
        var normal = new Vector(-direction.Y, direction.X);
        var p1 = tip - direction * ArrowSize + normal * (ArrowSize * 0.6);
        var p2 = tip - direction * ArrowSize - normal * (ArrowSize * 0.6);
        return new[] { tip, p1, p2 };
    }

    public double CalculateParallelOffset(int edgeIndex, int totalEdges)
    {
        if (totalEdges <= 1) return 0;
        double step = 14;
        double start = -(totalEdges - 1) * step / 2;
        return start + edgeIndex * step;
    }
}