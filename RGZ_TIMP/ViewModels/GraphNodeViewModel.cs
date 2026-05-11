using System.Windows.Input;
using RGZ_TIMP.Infrastructure;
using RGZ_TIMP.Models;

namespace RGZ_TIMP.ViewModels;

public sealed class GraphNodeViewModel : BaseViewModel
{
    private bool _isHighlighted;
    private bool _isStartHighlighted;

    public GraphNodeModel Model { get; }
    public int Number => Model.Number;
    public double Radius => Model.Radius;

    public string NodeCode
    {
        get => Model.NodeCode;
        set { if (Model.NodeCode != value) { Model.NodeCode = value; OnPropertyChanged(); } }
    }

    public double X
    {
        get => Model.CenterX;
        set { if (Model.CenterX != value) { Model.CenterX = value; OnPropertyChanged(); } }
    }

    public double Y
    {
        get => Model.CenterY;
        set { if (Model.CenterY != value) { Model.CenterY = value; OnPropertyChanged(); } }
    }

    public bool IsHighlighted
    {
        get => _isHighlighted;
        set => SetProperty(ref _isHighlighted, value);
    }

    public bool IsStartHighlighted
    {
        get => _isStartHighlighted;
        set => SetProperty(ref _isStartHighlighted, value);
    }

    public ICommand DeleteCommand { get; }
    public ICommand EditCommand { get; }

    public GraphNodeViewModel(GraphNodeModel model,
        Action<GraphNodeViewModel>? onDelete = null,
        Action<GraphNodeViewModel>? onEdit = null,
        Func<bool>? canEdit = null)
    {
        Model = model;
        DeleteCommand = new RelayCommand(_ => onDelete?.Invoke(this), _ => canEdit?.Invoke() ?? true);
        EditCommand = new RelayCommand(_ => onEdit?.Invoke(this), _ => canEdit?.Invoke() ?? true);
    }
}