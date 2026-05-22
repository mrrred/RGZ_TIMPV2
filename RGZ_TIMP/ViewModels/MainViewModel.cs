using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using RGZ_TIMP.Infrastructure;
using RGZ_TIMP.Models;
using RGZ_TIMP.Services;

namespace RGZ_TIMP.ViewModels;

public sealed class MainViewModel : BaseViewModel
{
    private readonly IGraphSerializationService _serializer;
    private readonly IAnimationService _animationService;
    private readonly IGraphGeometryService _geometryService;
    private readonly IDialogService _dialogService;
    private CancellationTokenSource? _animationCts;

    private readonly Stack<GraphProjectModel> _undoStack = new();
    private readonly Stack<GraphProjectModel> _redoStack = new();
    private bool _isRestoringSnapshot = false;

    private string _modeText = "Редактирование";
    public string ModeText
    {
        get => _modeText;
        set => SetProperty(ref _modeText, value);
    }

    private int _nextNodeId = 1;
    private int _nextEdgeId = 1;

    private int _animationDurationSeconds = 20;
    public int AnimationDurationSeconds
    {
        get => _animationDurationSeconds;
        set => SetProperty(ref _animationDurationSeconds, value);
    }

    private int _defaultEdgeDelayMs = 2000;
    public int DefaultEdgeDelayMs
    {
        get => _defaultEdgeDelayMs;
        set => SetProperty(ref _defaultEdgeDelayMs, value);
    }

    // preview line properties
    private bool _isConnecting;
    public bool IsConnecting { get => _isConnecting; set => SetProperty(ref _isConnecting, value); }

    private bool _hasProject;
    public bool HasProject { get => _hasProject; set => SetProperty(ref _hasProject, value); }

    private string? _currentProjectPath;

    private double _previewX1, _previewY1, _previewX2, _previewY2;
    public double PreviewX1 { get => _previewX1; set => SetProperty(ref _previewX1, value); }
    public double PreviewY1 { get => _previewY1; set => SetProperty(ref _previewY1, value); }
    public double PreviewX2 { get => _previewX2; set => SetProperty(ref _previewX2, value); }
    public double PreviewY2 { get => _previewY2; set => SetProperty(ref _previewY2, value); }

    private GraphNodeViewModel? _connectionSource;

    public ObservableCollection<GraphNodeViewModel> Nodes { get; }
    public ObservableCollection<GraphEdgeViewModel> Edges { get; }

    public ICommand AddNodeCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand NewProjectCommand { get; }
    public ICommand RunAnimationCommand { get; }
    public ICommand StopAnimationCommand { get; }
    public ICommand AnimationSettingsCommand { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }

    private Point _lastRightClickPosition;

    public MainViewModel()
    {
        _serializer = new GraphSerializationService();
        var interpreter = new RandomNodeCodeInterpreterService();
        _animationService = new AnimationService(interpreter);
        _geometryService = new GraphGeometryService();
        _dialogService = new DialogService();

        Nodes = new ObservableCollection<GraphNodeViewModel>();
        Edges = new ObservableCollection<GraphEdgeViewModel>();

        AddNodeCommand = new RelayCommand(_ => AddNodeAtLastRightClick(), _ => HasProject && !IsAnimating);
        SaveCommand = new RelayCommand(_ => SaveProject(), _ => HasProject && !IsAnimating);
        LoadCommand = new RelayCommand(_ => LoadProject(), _ => !IsAnimating);
        NewProjectCommand = new RelayCommand(_ => NewProject(), _ => !IsAnimating);
        RunAnimationCommand = new RelayCommand(async _ => await RunAnimation(), _ => !_isAnimating && HasProject && Nodes.Count > 0);
        StopAnimationCommand = new RelayCommand(_ => StopAnimation(), _ => _isAnimating);
        AnimationSettingsCommand = new RelayCommand(_ => ShowAnimationSettings(), _ => HasProject && !IsAnimating);
        ShowHelpCommand = new RelayCommand(_ => _dialogService.ShowHelpDialog());
        UndoCommand = new RelayCommand(_ => Undo(), _ => !IsAnimating && _undoStack.Count > 0);
        RedoCommand = new RelayCommand(_ => Redo(), _ => !IsAnimating && _redoStack.Count > 0);
    }

    public ICommand ShowHelpCommand { get; }

    private bool _isAnimating;
    public bool IsAnimating
    {
        get => _isAnimating;
        set
        {
            if (SetProperty(ref _isAnimating, value))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public void SetLastRightClickPosition(Point pos) => _lastRightClickPosition = pos;

    public void SaveSnapshot()
    {
        if (_isRestoringSnapshot || !HasProject) return;

        var snapshot = CreateProjectModel();
        _undoStack.Push(snapshot);
        _redoStack.Clear();
        CommandManager.InvalidateRequerySuggested();
    }

    private GraphProjectModel CreateProjectModel()
    {
        return new GraphProjectModel
        {
            AnimationDurationSeconds = AnimationDurationSeconds,
            DefaultEdgeDelaySeconds = DefaultEdgeDelayMs / 1000,
            NextNodeId = _nextNodeId,
            NextEdgeId = _nextEdgeId,
            Nodes = Nodes.Select(vm => new GraphNodeModel 
            { 
                Number = vm.Number, 
                CenterX = vm.X, 
                CenterY = vm.Y, 
                Radius = vm.Radius, 
                NodeCode = vm.NodeCode 
            }).ToList(),
            Edges = Edges.Select(vm => new GraphEdgeModel 
            { 
                EdgeId = vm.Model.EdgeId, 
                LocalOrder = vm.Model.LocalOrder, 
                SourceNodeNumber = vm.SourceNodeNumber, 
                TargetNodeNumber = vm.TargetNodeNumber, 
                StartX = vm.Model.StartX, 
                StartY = vm.Model.StartY, 
                EndX = vm.Model.EndX, 
                EndY = vm.Model.EndY, 
                Predicate = vm.Predicate, 
                DelaySeconds = vm.DelaySeconds, 
                ParallelOffset = vm.ParallelOffset 
            }).ToList()
        };
    }

    private void Undo()
    {
        if (_undoStack.Count == 0) return;
        var current = CreateProjectModel();
        _redoStack.Push(current);
        var snapshot = _undoStack.Pop();
        RestoreSnapshot(snapshot);
    }

    private void Redo()
    {
        if (_redoStack.Count == 0) return;
        var current = CreateProjectModel();
        _undoStack.Push(current);
        var snapshot = _redoStack.Pop();
        RestoreSnapshot(snapshot);
    }

    private void RestoreSnapshot(GraphProjectModel project)
    {
        _isRestoringSnapshot = true;
        try
        {
            Nodes.Clear();
            Edges.Clear();
            _nextNodeId = project.NextNodeId;
            _nextEdgeId = project.NextEdgeId;
            AnimationDurationSeconds = project.AnimationDurationSeconds;
            DefaultEdgeDelayMs = project.DefaultEdgeDelaySeconds * 1000;

            var nodeMap = new Dictionary<int, GraphNodeViewModel>();
            foreach (var nodeModel in project.Nodes)
            {
                var vm = new GraphNodeViewModel(nodeModel, onDelete: RemoveNode, onEdit: EditNodeCode, canEdit: () => !IsAnimating);
                Nodes.Add(vm);
                nodeMap[nodeModel.Number] = vm;
            }
            foreach (var edgeModel in project.Edges)
            {
                var from = nodeMap[edgeModel.SourceNodeNumber];
                var to = nodeMap[edgeModel.TargetNodeNumber];
                var edgeVm = new GraphEdgeViewModel(edgeModel, from, to, _geometryService, onEdit: EditEdge, onDelete: RemoveEdge, canEdit: () => !IsAnimating);
                Edges.Add(edgeVm);
            }
            foreach (var edge in Edges)
            {
                if (Nodes.FirstOrDefault(n => n.Number == edge.SourceNodeNumber) is GraphNodeViewModel n1 &&
                    Nodes.FirstOrDefault(n => n.Number == edge.TargetNodeNumber) is GraphNodeViewModel n2)
                    UpdateParallelOffsets(n1, n2);
            }
        }
        finally
        {
            _isRestoringSnapshot = false;
        }
    }

    private void AddNodeAtLastRightClick()
    {
        SaveSnapshot();
        AddNode(_lastRightClickPosition.X, _lastRightClickPosition.Y);
    }

    public void AddNode(double x, double y)
    {
        int newId = 1;
        var existingIds = Nodes.Select(n => n.Number).OrderBy(id => id).ToList();
        foreach (var id in existingIds)
        {
            if (id == newId) newId++;
            else break;
        }

        var model = new GraphNodeModel
        {
            Number = newId,
            CenterX = x - 40, // compensate for offset in canvas (size 80x80)
            CenterY = y - 40
        };
        var vm = new GraphNodeViewModel(model, onDelete: RemoveNode, onEdit: EditNodeCode, canEdit: () => !IsAnimating);
        Nodes.Add(vm);
    }

    public void ConnectNodes(GraphNodeViewModel from, GraphNodeViewModel to)
    {
        SaveSnapshot();
        if (from == to) // allow self-loop
        {
            var localOrder = Edges.Count(e => e.SourceNodeNumber == from.Number) + 1;
            var model = new GraphEdgeModel
            {
                EdgeId = _nextEdgeId++,
                LocalOrder = localOrder,
                SourceNodeNumber = from.Number,
                TargetNodeNumber = to.Number,
                Predicate = localOrder,
                DelaySeconds = _defaultEdgeDelayMs / 1000
            };
            var edgeVm = new GraphEdgeViewModel(model, from, to, _geometryService, onEdit: EditEdge, onDelete: RemoveEdge, canEdit: () => !IsAnimating);
            Edges.Add(edgeVm);
        }
        else
        {
            if (Edges.Any(e => e.SourceNodeNumber == from.Number && e.TargetNodeNumber == to.Number))
                return; // duplicate

            var localOrder = Edges.Count(e => e.SourceNodeNumber == from.Number) + 1;
            var model = new GraphEdgeModel
            {
                EdgeId = _nextEdgeId++,
                LocalOrder = localOrder,
                SourceNodeNumber = from.Number,
                TargetNodeNumber = to.Number,
                Predicate = localOrder,
                DelaySeconds = _defaultEdgeDelayMs / 1000
            };
            var edgeVm = new GraphEdgeViewModel(model, from, to, _geometryService, onEdit: EditEdge, onDelete: RemoveEdge, canEdit: () => !IsAnimating);
            Edges.Add(edgeVm);
        }
        UpdateParallelOffsets(from, to);
    }

    private void UpdateParallelOffsets(GraphNodeViewModel from, GraphNodeViewModel to)
    {
        var min = Math.Min(from.Number, to.Number);
        var max = Math.Max(from.Number, to.Number);

        var list = Edges.Where(e =>
            (Math.Min(e.SourceNodeNumber, e.TargetNodeNumber) == min) &&
            (Math.Max(e.SourceNodeNumber, e.TargetNodeNumber) == max))
            .OrderBy(e => e.SourceNodeNumber).ThenBy(e => e.EdgeId).ToList();

        for (int i = 0; i < list.Count; i++)
        {
            var edge = list[i];
            double step = 14;
            double absoluteShift = (i - (list.Count - 1) / 2.0) * step;

            if (edge.SourceNodeNumber == min)
                edge.ParallelOffset = absoluteShift;
            else
                edge.ParallelOffset = -absoluteShift;
        }
    }

    public void RemoveNode(GraphNodeViewModel node)
    {
        SaveSnapshot();
        var edgesToRemove = Edges.Where(e => e.SourceNodeNumber == node.Number || e.TargetNodeNumber == node.Number).ToList();
        var sourcesToUpdate = edgesToRemove.Select(e => e.SourceNodeNumber).Distinct().Where(id => id != node.Number).ToList();

        foreach (var edge in edgesToRemove)
            Edges.Remove(edge);

        foreach (var src in sourcesToUpdate)
        {
            var remaining = Edges.Where(e => e.SourceNodeNumber == src).OrderBy(e => e.LocalOrder).ToList();
            for (int i = 0; i < remaining.Count; i++)
                remaining[i].LocalOrder = i + 1;
        }

        Nodes.Remove(node);
    }

    private void RemoveEdge(GraphEdgeViewModel edge)
    {
        SaveSnapshot();
        int src = edge.SourceNodeNumber;
        var from = Nodes.FirstOrDefault(n => n.Number == edge.SourceNodeNumber);
        var to = Nodes.FirstOrDefault(n => n.Number == edge.TargetNodeNumber);

        Edges.Remove(edge);

        var remaining = Edges.Where(e => e.SourceNodeNumber == src).OrderBy(e => e.LocalOrder).ToList();
        for (int i = 0; i < remaining.Count; i++)
            remaining[i].LocalOrder = i + 1;

        if (from != null && to != null)
            UpdateParallelOffsets(from, to);
    }

    private void EditNodeCode(GraphNodeViewModel node)
    {
        var result = _dialogService.ShowNodeCodeDialog(node.NodeCode);
        if (result != null && result != node.NodeCode)
        {
            SaveSnapshot();
            node.NodeCode = result;
        }
    }

    private void EditEdge(GraphEdgeViewModel edge)
    {
        var result = _dialogService.ShowEdgeDialog(edge.Predicate, edge.DelaySeconds);
        if (result.HasValue && (result.Value.predicate != edge.Predicate || result.Value.delay != edge.DelaySeconds))
        {
            SaveSnapshot();
            edge.Predicate = result.Value.predicate;
            edge.DelaySeconds = result.Value.delay;
        }
    }

    private void ShowAnimationSettings()
    {
        var result = _dialogService.ShowAnimationSettingsDialog(AnimationDurationSeconds);
        if (result.HasValue && result.Value != AnimationDurationSeconds)
        {
            SaveSnapshot();
            AnimationDurationSeconds = result.Value;
        }
    }

    private async Task RunAnimation()
    {
        if (IsAnimating) return;

        // Validation for graph connection
        if (Nodes.Count > 0)
        {
            foreach (var node in Nodes)
            {
                bool hasIncoming = Edges.Any(e => e.TargetNodeNumber == node.Number);
                bool hasOutgoing = Edges.Any(e => e.SourceNodeNumber == node.Number);

                if (!hasIncoming && !hasOutgoing)
                {
                    MessageBox.Show($"Граф несвязный: узел {node.Number} не имеет входящих или выходящих дуг.", "Ошибка анимации", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        IsAnimating = true;
        _animationCts = new CancellationTokenSource();
        ModeText = "Анимация";
        try
        {
            await _animationService.RunAsync(Nodes.ToList(), Edges.ToList(),
                TimeSpan.FromSeconds(AnimationDurationSeconds), _animationCts.Token);
        }
        finally
        {
            StopAnimation();
        }
    }

    private void StopAnimation()
    {
        _animationCts?.Cancel();
        _animationCts = null;
        IsAnimating = false;
        foreach (var node in Nodes)
        {
            node.IsHighlighted = false;
            node.IsStartHighlighted = false;
        }
        ModeText = "Редактирование";
    }

    private void SaveProject()
    {
        if (_currentProjectPath == null || !HasProject) return;

        var project = CreateProjectModel();
        _serializer.Save(_currentProjectPath, project);
        _undoStack.Clear();
        _redoStack.Clear();
        CommandManager.InvalidateRequerySuggested();
    }

    private void LoadProject()
    {
        var path = _dialogService.ShowOpenFileDialog();
        if (path == null) return;

        try
        {
            var project = _serializer.Load(path);

            StopAnimation();
            Nodes.Clear();
            Edges.Clear();

            _currentProjectPath = path;
            HasProject = true;
            _undoStack.Clear();
            _redoStack.Clear();

            _nextNodeId = project.NextNodeId;
            _nextEdgeId = project.NextEdgeId;
            AnimationDurationSeconds = project.AnimationDurationSeconds;
            DefaultEdgeDelayMs = project.DefaultEdgeDelaySeconds * 1000;

            var nodeMap = new Dictionary<int, GraphNodeViewModel>();
            foreach (var nodeModel in project.Nodes)
            {
                var vm = new GraphNodeViewModel(nodeModel, onDelete: RemoveNode, onEdit: EditNodeCode, canEdit: () => !IsAnimating);
                Nodes.Add(vm);
                nodeMap[nodeModel.Number] = vm;
            }
            foreach (var edgeModel in project.Edges)
            {
                var from = nodeMap[edgeModel.SourceNodeNumber];
                var to = nodeMap[edgeModel.TargetNodeNumber];
                var edgeVm = new GraphEdgeViewModel(edgeModel, from, to, _geometryService, onEdit: EditEdge, onDelete: RemoveEdge, canEdit: () => !IsAnimating);
                Edges.Add(edgeVm);
            }
            // Recalculate all parallel offsets
            foreach (var edge in Edges)
                UpdateParallelOffsets(Nodes.First(n => n.Number == edge.SourceNodeNumber),
                                      Nodes.First(n => n.Number == edge.TargetNodeNumber));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось загрузить проект. Файл поврежден или имеет неверный формат.\n\nОшибка: {ex.Message}", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void NewProject()
    {
        var path = _dialogService.ShowSaveFileDialog("graph.xml");
        if (path == null) return;

        StopAnimation();
        Nodes.Clear();
        Edges.Clear();

        _currentProjectPath = path;
        HasProject = true;
        _nextNodeId = 1;
        _nextEdgeId = 1;
        AnimationDurationSeconds = 20;
        DefaultEdgeDelayMs = 2000;
        ModeText = "Редактирование";
        SaveProject();
    }

    public void StartConnection(GraphNodeViewModel source, Point startPoint)
    {
        _connectionSource = source;
        IsConnecting = true;
        PreviewX1 = startPoint.X;
        PreviewY1 = startPoint.Y;
        PreviewX2 = startPoint.X;
        PreviewY2 = startPoint.Y;
    }

    public void UpdatePreview(Point current)
    {
        PreviewX2 = current.X;
        PreviewY2 = current.Y;
    }

    public void CompleteConnection(GraphNodeViewModel? target)
    {
        if (_connectionSource != null && target != null)
            ConnectNodes(_connectionSource, target);
        CancelConnection();
    }

    public void CancelConnection()
    {
        _connectionSource = null;
        IsConnecting = false;
    }
}