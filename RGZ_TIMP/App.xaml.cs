using System.Configuration;
using System.Data;
using System.Windows;
using RGZ_TIMP.Views;
using RGZ_TIMP.ViewModels;
using RGZ_TIMP.Services;

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

            var dialogService = new DialogService();

            DispatcherUnhandledException += (s, args) =>
            {
                dialogService.ShowError($"Произошла непредвиденная ошибка:\n{args.Exception.Message}", "Ошибка приложения");
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
