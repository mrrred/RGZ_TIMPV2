namespace RGZ_TIMP.Models;

public sealed class GraphNodeModel
{
    public int Number { get; set; }
    public double CenterX { get; set; }
    public double CenterY { get; set; }
    public double Radius { get; set; } = 30;
    public string NodeCode { get; set; } = "rnd(1..n)";
}