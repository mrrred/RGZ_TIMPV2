using RGZ_TIMP.ViewModels;

namespace RGZ_TIMP.Services;

public interface IAnimationService
{
    Task RunAsync(
        IReadOnlyList<GraphNodeViewModel> nodes,
        IReadOnlyList<GraphEdgeViewModel> edges,
        TimeSpan duration,
        CancellationToken token);
}