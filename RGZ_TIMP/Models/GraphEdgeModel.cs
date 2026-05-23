namespace RGZ_TIMP.Models;

/// <summary>
/// Модель для представления ребра графа.
/// </summary>
public sealed class GraphEdgeModel
{
    /// <summary>
    /// Уникальный идентификатор ребра.
    /// </summary>
    public int EdgeId { get; set; }

    /// <summary>
    /// Локальный порядок обхода ребра.
    /// </summary>
    public int LocalOrder { get; set; }

    /// <summary>
    /// Номер исходного узла.
    /// </summary>
    public int SourceNodeNumber { get; set; }

    /// <summary>
    /// Номер целевого узла.
    /// </summary>
    public int TargetNodeNumber { get; set; }

    /// <summary>
    /// Начальная координата X.
    /// </summary>
    public double StartX { get; set; }

    /// <summary>
    /// Начальная координата Y.
    /// </summary>
    public double StartY { get; set; }

    /// <summary>
    /// Конечная координата X.
    /// </summary>
    public double EndX { get; set; }

    /// <summary>
    /// Конечная координата Y.
    /// </summary>
    public double EndY { get; set; }

    /// <summary>
    /// Условие (предикат) перехода.
    /// </summary>
    public int Predicate { get; set; }

    /// <summary>
    /// Задержка анимации в секундах.
    /// </summary>
    public int DelaySeconds { get; set; } = 2;

    /// <summary>
    /// Смещение ребра при наличии нескольких параллельных.
    /// </summary>
    public double ParallelOffset { get; set; }
}