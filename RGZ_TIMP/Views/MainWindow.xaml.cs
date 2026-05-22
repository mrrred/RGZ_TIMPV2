using System.Windows;
using System.Windows.Input;
using RGZ_TIMP.ViewModels;
using RGZ_TIMP.Views.Controls;

namespace RGZ_TIMP.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            if (vm.IsAnimating)
            {
                e.Handled = true;
                return;
            }

            var pos = e.GetPosition((IInputElement)sender);
            vm.SetLastRightClickPosition(pos);
        }
    }

    private void Canvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.IsConnecting)
        {
            var pos = e.GetPosition((IInputElement)sender);
            vm.UpdatePreview(pos);
        }
    }

    private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm && vm.IsConnecting)
        {
            // try to find a node under mouse to complete connection
            var pos = e.GetPosition((IInputElement)sender);
            var targetNode = FindNodeAt(pos);
            vm.CompleteConnection(targetNode);
        }
    }

    private GraphNodeViewModel? FindNodeAt(Point position)
    {
        // simple hit‑test: iterate through nodes and check bounding box
        if (DataContext is MainViewModel vm)
        {
            foreach (var node in vm.Nodes)
            {
                var rect = new Rect(node.X, node.Y, 80, 80); // узлы теперь размером 80x80 (визуально круг 60)
                if (rect.Contains(position))
                    return node;
            }
        }
        return null;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F1)
        {
            // Пытаемся найти элемент, над которым мышь, или сфокусированный элемент
            DependencyObject? target = Mouse.DirectlyOver as DependencyObject ?? Keyboard.FocusedElement as DependencyObject;

            string? kw = null;
            while (target != null)
            {
                kw = RGZ_TIMP.Services.HelpProvider.GetHelpKeyword(target);
                if (!string.IsNullOrEmpty(kw))
                    break;

                // Перемещаемся вверх по дереву
                if (target is FrameworkElement fe && fe.Parent != null)
                    target = fe.Parent;
                else
                    target = System.Windows.Media.VisualTreeHelper.GetParent(target);
            }

            if (!string.IsNullOrEmpty(kw))
            {
                RGZ_TIMP.Services.HelpProvider.ShowHelp(kw);
            }
            else
            {
                RGZ_TIMP.Services.HelpProvider.ShowHelp();
            }
            e.Handled = true;
        }
    }
}