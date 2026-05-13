using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using RGZ_TIMP.Infrastructure;
using RGZ_TIMP.Models;
using RGZ_TIMP.Services;

namespace RGZ_TIMP.ViewModels;

public sealed class GraphEdgeViewModel : BaseViewModel
{
    private readonly IGraphGeometryService _geometryService;
    private readonly GraphNodeViewModel _from;
    private readonly GraphNodeViewModel _to;

    private Geometry? _geometry;
    public Geometry? Geometry
    {
        get => _geometry;
        set => SetProperty(ref _geometry, value);
    }

    private PointCollection? _arrowPoints;
    public PointCollection? ArrowPoints
    {
        get => _arrowPoints;
        set => SetProperty(ref _arrowPoints, value);
    }

    public GraphEdgeModel Model { get; }
    public int EdgeId => Model.EdgeId;
    public int SourceNodeNumber => Model.SourceNodeNumber;
    public int TargetNodeNumber => Model.TargetNodeNumber;

    public int LocalOrder
    {
        get => Model.LocalOrder;
        set { if (Model.LocalOrder != value) { Model.LocalOrder = value; OnPropertyChanged(); } }
    }

    public int Predicate
    {
        get => Model.Predicate;
        set { if (Model.Predicate != value) { Model.Predicate = value; OnPropertyChanged(); UpdateGeometry(); } }
    }

    public int DelaySeconds
    {
        get => Model.DelaySeconds;
        set { if (Model.DelaySeconds != value) { Model.DelaySeconds = value; OnPropertyChanged(); } }
    }

    public double ParallelOffset
    {
        get => Model.ParallelOffset;
        set { if (Model.ParallelOffset != value) { Model.ParallelOffset = value; OnPropertyChanged(); UpdateGeometry(); } }
    }

    public ICommand DeleteCommand { get; }
    public ICommand EditCommand { get; }

    public GraphEdgeViewModel(GraphEdgeModel model,
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