namespace RGZ_TIMP.Models;

/// <summary>
/// Модель проекта графа.
/// </summary>
public sealed class GraphProjectModel
{
    /// <summary>
    /// Длительность анимации в секундах.
    /// </summary>
    public int AnimationDurationSeconds { get; set; } = 20;

    /// <summary>
    /// Длительность задержки на ребрах по умолчанию (в секундах).
    /// </summary>
    public int DefaultEdgeDelaySeconds { get; set; } = 2;

    /// <summary>
    /// Идентификатор следующего узла.
    /// </summary>
    public int NextNodeId { get; set; } = 1;

    /// <summary>
    /// Идентификатор следующего ребра.
    /// </summary>
    public int NextEdgeId { get; set; } = 1;

    /// <summary>
    /// Список узлов графа.
    /// </summary>
    public List<GraphNodeModel> Nodes { get; set; } = new();

    /// <summary>
    /// Список ребер графа.
    /// </summary>
    public List<GraphEdgeModel> Edges { get; set; } = new();
}