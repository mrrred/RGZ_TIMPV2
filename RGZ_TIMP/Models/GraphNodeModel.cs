namespace RGZ_TIMP.Models;

/// <summary>
/// Модель для представления узла графа.
/// </summary>
public sealed class GraphNodeModel
{
    /// <summary>
    /// Номер узла.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Координата X центра узла.
    /// </summary>
    public double CenterX { get; set; }

    /// <summary>
    /// Координата Y центра узла.
    /// </summary>
    public double CenterY { get; set; }

    /// <summary>
    /// Радиус узла.
    /// </summary>
    public double Radius { get; set; } = 30;

    /// <summary>
    /// Код, выполняемый в узле.
    /// </summary>
    public string NodeCode { get; set; } = "return Random.Next(1, n + 1);";
}