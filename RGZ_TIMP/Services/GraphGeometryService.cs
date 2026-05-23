using System.Windows;
using System.Windows.Media;

namespace RGZ_TIMP.Services;

/// <summary>
/// Сервис для геометрических вычислений.
/// </summary>
public sealed class GraphGeometryService : IGraphGeometryService
{
    private const double ArrowSize = 10;

    /// <summary>
    /// Вычисляет начальную точку отрисовки стрелки от узла с учетом радиуса и смещения.
    /// </summary>
    /// <param name="from">Точка центра исходного узла.</param>
    /// <param name="to">Точка центра целевого узла.</param>
    /// <param name="radius">Радиус узла.</param>
    /// <param name="parallelOffset">Отклонение для параллельных ребер.</param>
    /// <returns>Точка начала ребра.</returns>
    public Point GetEdgeStart(Point from, Point to, double radius, double parallelOffset)
    {
        var direction = to - from;
        if (direction.Length < 0.001)
        {
            return from;
        }

        direction.Normalize();
        var normal = new Vector(-direction.Y, direction.X);
        var radial = Math.Sqrt(Math.Max(0, radius * radius - Math.Min(parallelOffset * parallelOffset, radius * radius - 1)));

        return from + direction * radial + normal * parallelOffset;
    }

    /// <summary>
    /// Вычисляет конечную точку отрисовки стрелки к узлу с учетом радиуса и смещения.
    /// </summary>
    /// <param name="from">Точка центра исходного узла.</param>
    /// <param name="to">Точка центра целевого узла.</param>
    /// <param name="radius">Радиус узла.</param>
    /// <param name="parallelOffset">Отклонение для параллельных ребер.</param>
    /// <returns>Точка конца ребра.</returns>
    public Point GetEdgeEnd(Point from, Point to, double radius, double parallelOffset)
    {
        var direction = to - from;
        if (direction.Length < 0.001)
        {
            return to;
        }

        direction.Normalize();
        var normal = new Vector(-direction.Y, direction.X);
        var radial = Math.Sqrt(Math.Max(0, radius * radius - Math.Min(parallelOffset * parallelOffset, radius * radius - 1)));

        return to - direction * radial + normal * parallelOffset;
    }

    /// <summary>
    /// Создает массив точек, образующих полигон стрелки.
    /// </summary>
    /// <param name="tip">Точка острия.</param>
    /// <param name="direction">Вектор направления стрелки.</param>
    /// <returns>Массив точек.</returns>
    public Point[] BuildArrow(Point tip, Vector direction)
    {
        direction.Normalize();
        var normal = new Vector(-direction.Y, direction.X);
        var p1 = tip - direction * ArrowSize + normal * (ArrowSize * 0.6);
        var p2 = tip - direction * ArrowSize - normal * (ArrowSize * 0.6);

        return new[] { tip, p1, p2 };
    }

    /// <summary>
    /// Рассчитывает смещение от центра линии для параллельных ребер.
    /// </summary>
    /// <param name="edgeIndex">Индекс параллельного ребра.</param>
    /// <param name="totalEdges">Общее количество параллельных ребер.</param>
    /// <returns>Величина смещения.</returns>
    public double CalculateParallelOffset(int edgeIndex, int totalEdges)
    {
        if (totalEdges <= 1)
        {
            return 0;
        }

        double step = 14;
        double start = -(totalEdges - 1) * step / 2;

        return start + edgeIndex * step;
    }
}