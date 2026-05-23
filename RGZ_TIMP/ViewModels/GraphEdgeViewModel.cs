using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using RGZ_TIMP.Infrastructure;
using RGZ_TIMP.Models;
using RGZ_TIMP.Services;

namespace RGZ_TIMP.ViewModels;

/// <summary>
/// Модель представления для ребра графа.
/// </summary>
public sealed class GraphEdgeViewModel : BaseViewModel
{
    private readonly IGraphGeometryService _geometryService;
    private readonly GraphNodeViewModel _from;
    private readonly GraphNodeViewModel _to;

    private Geometry? _geometry;
    private PointCollection? _arrowPoints;

    /// <summary>
    /// Геометрия линии ребра.
    /// </summary>
    public Geometry? Geometry
    {
        get
        {
            return _geometry;
        }
        set
        {
            SetProperty(ref _geometry, value);
        }
    }

    /// <summary>
    /// Точки для отрисовки стрелки на конце ребра.
    /// </summary>
    public PointCollection? ArrowPoints
    {
        get
        {
            return _arrowPoints;
        }
        set
        {
            SetProperty(ref _arrowPoints, value);
        }
    }

    /// <summary>
    /// Исходная модель ребра.
    /// </summary>
    public GraphEdgeModel Model { get; }

    /// <summary>
    /// Идентификатор ребра.
    /// </summary>
    public int EdgeId
    {
        get
        {
            return Model.EdgeId;
        }
    }

    /// <summary>
    /// Номер исходного узла.
    /// </summary>
    public int SourceNodeNumber
    {
        get
        {
            return Model.SourceNodeNumber;
        }
    }

    /// <summary>
    /// Номер целевого узла.
    /// </summary>
    public int TargetNodeNumber
    {
        get
        {
            return Model.TargetNodeNumber;
        }
    }

    /// <summary>
    /// Локальный порядок перехода.
    /// </summary>
    public int LocalOrder
    {
        get
        {
            return Model.LocalOrder;
        }
        set
        {
            if (Model.LocalOrder != value)
            {
                Model.LocalOrder = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Предикат (условие перехода).
    /// </summary>
    public int Predicate
    {
        get
        {
            return Model.Predicate;
        }
        set
        {
            if (Model.Predicate != value)
            {
                Model.Predicate = value;
                OnPropertyChanged();
                UpdateGeometry();
            }
        }
    }

    /// <summary>
    /// Задержка анимации в секундах.
    /// </summary>
    public int DelaySeconds
    {
        get
        {
            return Model.DelaySeconds;
        }
        set
        {
            if (Model.DelaySeconds != value)
            {
                Model.DelaySeconds = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Смещение параллельного ребра.
    /// </summary>
    public double ParallelOffset
    {
        get
        {
            return Model.ParallelOffset;
        }
        set
        {
            if (Model.ParallelOffset != value)
            {
                Model.ParallelOffset = value;
                OnPropertyChanged();
                UpdateGeometry();
            }
        }
    }

    /// <summary>
    /// Команда для удаления ребра.
    /// </summary>
    public ICommand DeleteCommand { get; }

    /// <summary>
    /// Команда для изменения свойств ребра.
    /// </summary>
    public ICommand EditCommand { get; }

    /// <summary>
    /// Инициализирует новый экземпляр класса GraphEdgeViewModel.
    /// </summary>
    /// <param name="model">Модель ребра.</param>
    /// <param name="from">Исходный узел.</param>
    /// <param name="to">Целевой узел.</param>
    /// <param name="geometryService">Геометрический сервис.</param>
    /// <param name="onEdit">Обработчик редактирования.</param>
    /// <param name="onDelete">Обработчик удаления.</param>
    /// <param name="canEdit">Предикат доступности редактирования.</param>
    public GraphEdgeViewModel(
        GraphEdgeModel model,
        GraphNodeViewModel from,
        GraphNodeViewModel to,
        IGraphGeometryService geometryService,
        Action<GraphEdgeViewModel>? onEdit = null,
        Action<GraphEdgeViewModel>? onDelete = null,
        Func<bool>? canEdit = null)
    {
        Model = model;
        _from = from;
        _to = to;
        _geometryService = geometryService;

        _from.PropertyChanged += (_, _) => UpdateGeometry();
        _to.PropertyChanged += (_, _) => UpdateGeometry();

        UpdateGeometry();

        DeleteCommand = new RelayCommand(_ => onDelete?.Invoke(this), _ => canEdit?.Invoke() ?? true);
        EditCommand = new RelayCommand(_ => onEdit?.Invoke(this), _ => canEdit?.Invoke() ?? true);
    }

    /// <summary>
    /// Обновляет геометрию ребра (линию и стрелку).
    /// </summary>
    public void UpdateGeometry()
    {
        var fromCenter = new Point(_from.X + _from.Radius + 10, _from.Y + _from.Radius + 10);
        var toCenter = new Point(_to.X + _to.Radius + 10, _to.Y + _to.Radius + 10);

        if (ReferenceEquals(_from, _to))
        {
            BuildSelfLoop(fromCenter);
            return;
        }

        double offset = Model.ParallelOffset;
        var start = _geometryService.GetEdgeStart(fromCenter, toCenter, _from.Radius, offset);
        var end = _geometryService.GetEdgeEnd(fromCenter, toCenter, _to.Radius, offset);

        Geometry = new LineGeometry(start, end);
        var direction = end - start;
        ArrowPoints = new PointCollection(_geometryService.BuildArrow(end, direction));
    }

    private void BuildSelfLoop(Point center)
    {
        const double loopRadius = 18;
        var loopCenter = new Point(center.X + 24, center.Y - 24);
        var start = new Point(loopCenter.X + loopRadius, loopCenter.Y);
        var mid = new Point(loopCenter.X - loopRadius, loopCenter.Y);

        var figure = new PathFigure { StartPoint = start };
        figure.Segments.Add(new ArcSegment(mid, new Size(loopRadius, loopRadius), 0, false, SweepDirection.Clockwise, true));
        figure.Segments.Add(new ArcSegment(start, new Size(loopRadius, loopRadius), 0, false, SweepDirection.Clockwise, true));
        Geometry = new PathGeometry(new[] { figure });

        var angle = -Math.PI / 4;
        var tip = new Point(loopCenter.X + loopRadius * Math.Cos(angle), loopCenter.Y + loopRadius * Math.Sin(angle));
        var direction = new Vector(-Math.Sin(angle), Math.Cos(angle));

        direction.Normalize();
        var normal = new Vector(-direction.Y, direction.X);
        var arrowSize = 10;
        var p1 = tip - direction * arrowSize + normal * (arrowSize * 0.6);
        var p2 = tip - direction * arrowSize - normal * (arrowSize * 0.6);

        ArrowPoints = new PointCollection(new[] { tip, p1, p2 });
    }
}