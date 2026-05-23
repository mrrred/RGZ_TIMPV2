using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using RGZ_TIMP.ViewModels;

namespace RGZ_TIMP.Views.Controls;

/// <summary>
/// Представление для отображения узла графа.
/// </summary>
public partial class GraphNodeView : UserControl
{
    private bool _isDragging;
    private Point _dragOffset;
    private bool _isHandleDrag;

    /// <summary>
    /// Инициализирует новый экземпляр класса GraphNodeView.
    /// </summary>
    public GraphNodeView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Обрабатывает событие входа мыши.
    /// </summary>
    protected override void OnMouseEnter(MouseEventArgs e)
    {
        base.OnMouseEnter(e);
        
        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            UpdateHandlePosition(e.GetPosition(this));
        }
    }

    /// <summary>
    /// Обрабатывает событие выхода мыши.
    /// </summary>
    protected override void OnMouseLeave(MouseEventArgs e)
    {
        base.OnMouseLeave(e);
        
        if (!_isHandleDrag)
        {
            Handle.Visibility = Visibility.Collapsed;
        }
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var mainVm = Application.Current.MainWindow?.DataContext as MainViewModel;
        
        if (mainVm != null && mainVm.IsAnimating)
        {
            return;
        }

        if (DataContext is not GraphNodeViewModel vm)
        {
            return;
        }

        // Двойной клик – редактирование
        if (e.ClickCount == 2)
        {
            if (vm.EditCommand.CanExecute(null))
            {
                vm.EditCommand.Execute(null);
            }
            
            e.Handled = true;
            return;
        }

        // Обычное перетаскивание
        if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
        {
            _isDragging = true;
            _dragOffset = e.GetPosition(this);
            
            if (sender is UIElement el)
            {
                el.CaptureMouse();
            }
            
            e.Handled = true;
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var mainVm = Application.Current.MainWindow?.DataContext as MainViewModel;
        
        if (mainVm != null && mainVm.IsAnimating)
        {
            return;
        }

        if (_isDragging && e.LeftButton == MouseButtonState.Pressed && DataContext is GraphNodeViewModel vm)
        {
            var canvas = FindCanvas();
            
            if (canvas == null)
            {
                return;
            }

            var pos = e.GetPosition(canvas);
            vm.X = pos.X - _dragOffset.X;
            vm.Y = pos.Y - _dragOffset.Y;
            return;
        }
        else if (_isDragging)
        {
            _isDragging = false;
            
            if (sender is UIElement el)
            {
                el.ReleaseMouseCapture();
            }
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && !_isHandleDrag)
        {
            UpdateHandlePosition(e.GetPosition(this));
        }
        else if (!_isHandleDrag)
        {
            Handle.Visibility = Visibility.Collapsed;
        }
    }

    private void UpdateHandlePosition(Point mousePos)
    {
        var center = new Point(Width / 2, Height / 2);
        var vector = mousePos - center;
        
        if (vector.Length > 0)
        {
            vector.Normalize();
        }
        else
        {
            vector = new Vector(1, 0);
        }

        var boundary = center + vector * 30; // Radius=30, since ellipse is 60x60
        Handle.Margin = new Thickness(boundary.X - Handle.Width / 2, boundary.Y - Handle.Height / 2, 0, 0);
        Handle.Visibility = Visibility.Visible;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            
            if (sender is UIElement el)
            {
                el.ReleaseMouseCapture();
            }
        }
    }

    private void Handle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var mainVm = Application.Current.MainWindow?.DataContext as MainViewModel;
        
        if (mainVm == null || mainVm.IsAnimating)
        {
            return;
        }

        if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
        {
            return;
        }
        
        if (DataContext is not GraphNodeViewModel vm)
        {
            return;
        }

        _isHandleDrag = true;
        Handle.Visibility = Visibility.Visible;
        
        var startPoint = e.GetPosition(Application.Current.MainWindow?.Content as UIElement ?? this);
        // Translate to canvas coordinates (we'll get relative to the canvas later)
        mainVm.StartConnection(vm, e.GetPosition(FindCanvas()));
        
        if (sender is UIElement el)
        {
            el.CaptureMouse();
        }
        
        e.Handled = true;
    }

    private void Handle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isHandleDrag)
        {
            return;
        }
        
        _isHandleDrag = false;
        
        if (sender is UIElement el)
        {
            el.ReleaseMouseCapture();
        }
        
        var mainVm = Application.Current.MainWindow?.DataContext as MainViewModel;
        
        if (mainVm != null)
        {
            var canvas = FindCanvas();
            var pos = e.GetPosition(canvas);
            var targetNode = FindNodeAt(canvas, pos);
            mainVm.CompleteConnection(targetNode);
        }
    }

    private Canvas? FindCanvas()
    {
        DependencyObject? current = this;
        
        while (current != null && current is not Canvas)
        {
            current = VisualTreeHelper.GetParent(current);
        }
            
        return current as Canvas;
    }

    private GraphNodeViewModel? FindNodeAt(Canvas? canvas, Point pos)
    {
        var mainVm = Application.Current.MainWindow?.DataContext as MainViewModel;
        
        if (mainVm == null || canvas == null)
        {
            return null;
        }

        foreach (var node in mainVm.Nodes)
        {
            var rect = new Rect(node.X, node.Y, 80, 80);
            
            if (rect.Contains(pos))
            {
                return node;
            }
        }
        
        return null;
    }
}