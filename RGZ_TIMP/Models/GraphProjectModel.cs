namespace RGZ_TIMP.Models;

public sealed class GraphProjectModel
{
    public int AnimationDurationSeconds { get; set; } = 20;
    public int DefaultEdgeDelaySeconds { get; set; } = 2;
    public int NextNodeId { get; set; } = 1;
    public int NextEdgeId { get; set; } = 1;
    public List<GraphNodeModel> Nodes { get; set; } = new();
    public List<GraphEdgeModel> Edges { get; set; } = new();
}