using System.Configuration;
using System.Data;
using System.Windows;
using RGZ_TIMP.Views;
using RGZ_TIMP.ViewModels;

namespace RGZ_TIMP
{
    /// <summary>
    /// Логика взаимодействия для App.xaml.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Вызывается при запуске приложения.
        /// </summary>
        /// <param name="e">Аргументы события запуска.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"Произошла непредвиденная ошибка:\n{args.Exception.Message}", "Ошибка приложения", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            var mainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            mainWindow.Show();
        }
    }
}
