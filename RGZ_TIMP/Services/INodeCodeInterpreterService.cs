namespace RGZ_TIMP.Services;

/// <summary>
/// Итерфейс для интерпретатора кода узлов.
/// </summary>
public interface INodeCodeInterpreterService
{
    /// <summary>
    /// Оценивает код и возвращает предикат выбранного ребра.
    /// </summary>
    /// <param name="code">Исходный код, содержащийся в узле.</param>
    /// <param name="outgoingCount">Количество исходящих ребер.</param>
    /// <returns>Сгенерированное значение или номер ребра.</returns>
    int Evaluate(string code, int outgoingCount);
}