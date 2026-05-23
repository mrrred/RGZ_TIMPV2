using System.Windows.Input;
using RGZ_TIMP.Infrastructure;
using RGZ_TIMP.Models;

namespace RGZ_TIMP.ViewModels;

/// <summary>
/// Модель представления для узла графа.
/// </summary>
public sealed class GraphNodeViewModel : BaseViewModel
{
    private bool _isHighlighted;
    private bool _isStartHighlighted;

    /// <summary>
    /// Исходная модель узла.
    /// </summary>
    public GraphNodeModel Model { get; }

    /// <summary>
    /// Номер узла.
    /// </summary>
    public int Number
    {
        get
        {
            return Model.Number;
        }
    }

    /// <summary>
    /// Радиус узла для отрисовки.
    /// </summary>
    public double Radius
    {
        get
        {
            return Model.Radius;
        }
    }

    /// <summary>
    /// Код, привязанный к узлу.
    /// </summary>
    public string NodeCode
    {
        get
        {
            return Model.NodeCode;
        }
        set
        {
            if (Model.NodeCode != value)
            {
                Model.NodeCode = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Координата X центра узла.
    /// </summary>
    public double X
    {
        get
        {
            return Model.CenterX;
        }
        set
        {
            if (Model.CenterX != value)
            {
                Model.CenterX = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Координата Y центра узла.
    /// </summary>
    public double Y
    {
        get
        {
            return Model.CenterY;
        }
        set
        {
            if (Model.CenterY != value)
            {
                Model.CenterY = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Определяет, выделен ли узел (анимация обхода).
    /// </summary>
    public bool IsHighlighted
    {
        get
        {
            return _isHighlighted;
        }
        set
        {
            SetProperty(ref _isHighlighted, value);
        }
    }

    /// <summary>
    /// Определяет, является ли узел стартовым.
    /// </summary>
    public bool IsStartHighlighted
    {
        get
        {
            return _isStartHighlighted;
        }
        set
        {
            SetProperty(ref _isStartHighlighted, value);
        }
    }

    /// <summary>
    /// Команда для удаления узла.
    /// </summary>
    public ICommand DeleteCommand { get; }

    /// <summary>
    /// Команда для редактирования узла.
    /// </summary>
    public ICommand EditCommand { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса GraphNodeViewModel.
    /// </summary>
    /// <param name="model">Модель узла.</param>
    /// <param name="onDelete">Действие при удалении.</param>
    /// <param name="onEdit">Действие при редактировании.</param>
    /// <param name="canEdit">Условие возможности редактирования.</param>
    public GraphNodeViewModel(
        GraphNodeModel model,
        Action<GraphNodeViewModel>? onDelete = null,
        Action<GraphNodeViewModel>? onEdit = null,
        Func<bool>? canEdit = null)
    {
        Model = model;
        DeleteCommand = new RelayCommand(_ => onDelete?.Invoke(this), _ => canEdit?.Invoke() ?? true);
        EditCommand = new RelayCommand(_ => onEdit?.Invoke(this), _ => canEdit?.Invoke() ?? true);
    }
}