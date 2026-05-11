using System.Windows;
using System.Windows.Threading;
using RGZ_TIMP.ViewModels;

namespace RGZ_TIMP.Services;

public sealed class AnimationService : IAnimationService
{
    private readonly INodeCodeInterpreterService _interpreter;
    private readonly Dispatcher _dispatcher;

    public AnimationService(INodeCodeInterpreterService interpreter)
    {
        _interpreter = interpreter;
        _dispatcher = Application.Current.Dispatcher;
    }

    public async Task RunAsync(
        IReadOnlyList<GraphNodeViewModel> nodes,
        IReadOnlyList<GraphEdgeViewModel> edges,
        TimeSpan duration,
        CancellationToken token)
    {
        var startNode = nodes.FirstOrDefault(n => n.Number == 1);
        if (startNode == null) return;

        var current = startNode;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            while (!token.IsCancellationRequested && stopwatch.Elapsed < duration)
            {
                await _dispatcher.InvokeAsync(() =>
                {
                    current.IsStartHighlighted = true;
                });

                await Task.Delay(250, token);

                await _dispatcher.InvokeAsync(() =>
                {
                    current.IsStartHighlighted = false;
                    current.IsHighlighted = true;
                });

                var outgoing = edges
                    .Where(e => e.SourceNodeNumber == current.Number)
                    .OrderBy(e => e.Predicate)
                    .ToList();

                if (outgoing.Count == 0) break;

                var predicate = _interpreter.Evaluate(current.NodeCode, outgoing.Count);
                var selected = outgoing.FirstOrDefault(e => e.Predicate == predicate) ?? outgoing[0];

                await Task.Delay(TimeSpan.FromSeconds(selected.DelaySeconds), token);

                await _dispatcher.InvokeAsync(() =>
                {
                    current.IsHighlighted = false;
                    current = nodes.First(n => n.Number == selected.TargetNodeNumber);
                });
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when the animation is stopped
        }

        // cleanup
        await _dispatcher.InvokeAsync(() =>
        {
            foreach (var n in nodes)
            {
                n.IsHighlighted = false;
                n.IsStartHighlighted = false;
            }
        });
    }
}