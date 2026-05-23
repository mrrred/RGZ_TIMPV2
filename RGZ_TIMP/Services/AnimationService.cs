using System.Windows;
using System.Windows.Threading;
using RGZ_TIMP.ViewModels;

namespace RGZ_TIMP.Services;

/// <summary>
/// Сервис анимации обхода графа.
/// </summary>
public sealed class AnimationService : IAnimationService
{
    private readonly INodeCodeInterpreterService _interpreter;
    private readonly Dispatcher _dispatcher;

    /// <summary>
    /// Инициализирует новый экземпляр класса AnimationService.
    /// </summary>
    /// <param name="interpreter">Интерпретатор для вычисления переходов в узлах.</param>
    public AnimationService(INodeCodeInterpreterService interpreter)
    {
        _interpreter = interpreter;
        _dispatcher = Application.Current.Dispatcher;
    }

    /// <summary>
    /// Запускает анимацию обхода графа.
    /// </summary>
    /// <param name="nodes">Список моделей узлов.</param>
    /// <param name="edges">Список моделей ребер.</param>
    /// <param name="duration">Длительность работы анимации.</param>
    /// <param name="token">Токен отмены.</param>
    /// <returns>Задача, представляющая выполнение анимации.</returns>
    public async Task RunAsync(
        IReadOnlyList<GraphNodeViewModel> nodes,
        IReadOnlyList<GraphEdgeViewModel> edges,
        TimeSpan duration,
        CancellationToken token)
    {
        var startNode = nodes.FirstOrDefault(n => n.Number == 1);
        if (startNode == null)
        {
            return;
        }

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
                    .OrderBy(e => e.LocalOrder)
                    .ToList();

                if (outgoing.Count == 0)
                {
                    break;
                }

                var resultIndex = _interpreter.Evaluate(current.NodeCode, outgoing.Count);
                var selected = outgoing.ElementAtOrDefault(resultIndex - 1) ?? outgoing[0];

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
            // Ожидается при остановке анимации
        }

        // Очистка выделения
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