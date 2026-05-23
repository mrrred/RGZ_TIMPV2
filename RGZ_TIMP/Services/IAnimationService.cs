using RGZ_TIMP.ViewModels;

namespace RGZ_TIMP.Services;

/// <summary>
/// Интерфейс для сервиса анимации.
/// </summary>
public interface IAnimationService
{
    /// <summary>
    /// Запускает анимацию обхода графа.
    /// </summary>
    /// <param name="nodes">Список узлов.</param>
    /// <param name="edges">Список ребер.</param>
    /// <param name="duration">Длительность анимации.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, представляющая процесс анимации.</returns>
    Task RunAsync(
        IReadOnlyList<GraphNodeViewModel> nodes,
        IReadOnlyList<GraphEdgeViewModel> edges,
        TimeSpan duration,
        CancellationToken token);
}