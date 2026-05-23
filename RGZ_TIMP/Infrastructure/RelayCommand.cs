using System;
using System.Windows.Input;

namespace RGZ_TIMP.Infrastructure;

/// <summary>
/// Реализация ICommand для ретрансляции действий с помощью делегатов.
/// </summary>
public sealed class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    /// <summary>
    /// Инициализирует новый экземпляр класса RelayCommand.
    /// </summary>
    /// <param name="execute">Метод для выполнения.</param>
    /// <param name="canExecute">Метод, определяющий возможность выполнения команды.</param>
    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    /// <summary>
    /// Событие, возникающее при изменении возможности выполнения команды.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }

    /// <summary>
    /// Определяет, может ли команда выполняться в ее текущем состоянии.
    /// </summary>
    /// <param name="parameter">Параметр, используемый командой.</param>
    /// <returns>True, если команда может быть выполнена.</returns>
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke(parameter) ?? true;
    }

    /// <summary>
    /// Вызывает выполнение метода команды.
    /// </summary>
    /// <param name="parameter">Параметр, используемый командой.</param>
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    /// <summary>
    /// Принудительно вызывает обновление события CanExecuteChanged.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}