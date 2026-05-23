using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RGZ_TIMP.Infrastructure;

/// <summary>
/// Базовый класс для наблюдаемых объектов, реализующий INotifyPropertyChanged.
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    /// <summary>
    /// Событие, возникающее при изменении свойства.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Вызывает событие PropertyChanged.
    /// </summary>
    /// <param name="propertyName">Имя изменившегося свойства.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Устанавливает значение свойства и вызывает PropertyChanged, если значение изменилось.
    /// </summary>
    /// <typeparam name="T">Тип свойства.</typeparam>
    /// <param name="field">Ссылка на поле, хранящее значение свойства.</param>
    /// <param name="value">Новое значение.</param>
    /// <param name="propertyName">Имя свойства.</param>
    /// <returns>True, если значение было изменено, иначе False.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}