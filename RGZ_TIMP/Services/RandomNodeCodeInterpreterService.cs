namespace RGZ_TIMP.Services;

/// <summary>
/// Сервис интерпретатора кода узлов, использующий случайную генерацию.
/// </summary>
public sealed class RandomNodeCodeInterpreterService : INodeCodeInterpreterService
{
    private readonly Random _random = new();

    /// <summary>
    /// Оценивает код и возвращает случайный индекс исходящего ребра.
    /// </summary>
    /// <param name="code">Код, прикрепленный к узлу.</param>
    /// <param name="outgoingCount">Количество исходящих ребер.</param>
    /// <returns>Целое число в диапазоне от 1 до количества ребер.</returns>
    public int Evaluate(string code, int outgoingCount)
    {
        return _random.Next(1, outgoingCount + 1);
    }
}