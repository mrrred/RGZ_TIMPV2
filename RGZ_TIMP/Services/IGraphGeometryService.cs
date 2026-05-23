using System.Windows;
using System.Windows.Media;

namespace RGZ_TIMP.Services;

/// <summary>
/// Интерфейс для сервиса геометрических расчетов графа.
/// </summary>
public interface IGraphGeometryService
{
    /// <summary>
    /// Получает начальную точку ребра с учетом радиуса узла и смещения.
    /// </summary>
    /// <param name="from">Центр исходного узла.</param>
    /// <param name="to">Центр целевого узла.</param>
    /// <param name="radius">Радиус узла.</param>
    /// <param name="parallelOffset">Смещение параллельных ребер.</param>
    /// <returns>Начальная координата для отрисовки ребра.</returns>
    Point GetEdgeStart(Point from, Point to, double radius, double parallelOffset);

    /// <summary>
    /// Получает конечную точку ребра с учетом радиуса узла и смещения.
    /// </summary>
    /// <param name="from">Центр исходного узла.</param>
    /// <param name="to">Центр целевого узла.</param>
    /// <param name="radius">Радиус узла.</param>
    /// <param name="parallelOffset">Смещение параллельных ребер.</param>
    /// <returns>Конечная координата для отрисовки ребра.</returns>
    Point GetEdgeEnd(Point from, Point to, double radius, double parallelOffset);

    /// <summary>
    /// Строит набор точек, формирующих стрелку.
    /// </summary>
    /// <param name="tip">Конечная точка (острие стрелки).</param>
    /// <param name="direction">Вектор направления ребра.</param>
    /// <returns>Массив точек для полигона стрелки.</returns>
    Point[] BuildArrow(Point tip, Vector direction);

    /// <summary>
    /// Рассчитывает смещение для параллельных ребер.
    /// </summary>
    /// <param name="edgeIndex">Индекс текущего ребра.</param>
    /// <param name="totalEdges">Общее количество параллельных ребер.</param>
    /// <returns>Величина смещения.</returns>
    double CalculateParallelOffset(int edgeIndex, int totalEdges);
}