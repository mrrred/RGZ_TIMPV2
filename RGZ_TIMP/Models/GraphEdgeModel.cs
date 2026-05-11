namespace RGZ_TIMP.Models;

public sealed class GraphEdgeModel
{
    public int EdgeId { get; set; }
    public int LocalOrder { get; set; }
    public int SourceNodeNumber { get; set; }
    public int TargetNodeNumber { get; set; }
    public double StartX { get; set; }
    public double StartY { get; set; }
    public double EndX { get; set; }
    public double EndY { get; set; }
    public int Predicate { get; set; }
    public int DelaySeconds { get; set; } = 2;
    public double ParallelOffset { get; set; }
}