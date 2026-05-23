using System.Windows.Controls;
using System.Windows.Input;
using RGZ_TIMP.ViewModels;

namespace RGZ_TIMP.Views.Controls;

/// <summary>
/// Представление для отображения ребра графа.
/// </summary>
public partial class GraphEdgeView : UserControl
{
    /// <summary>
    /// Инициализирует новый экземпляр класса GraphEdgeView.
    /// </summary>
    public GraphEdgeView()
    {
        InitializeComponent();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2 && DataContext is GraphEdgeViewModel vm)
        {
            if (vm.EditCommand.CanExecute(null))
            {
                vm.EditCommand.Execute(null);
            }

            e.Handled = true;
        }
    }
}