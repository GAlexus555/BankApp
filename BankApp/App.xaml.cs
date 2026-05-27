using BankApp.ViewModels;
using BankApp.Stores;
using BankApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using Serilog;

namespace BankApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NavigationStore _navigationStore;

        /// <summary>
        /// Constructor
        /// </summary>
        public App()
        {
            // Loger erstellen
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Month, fileSizeLimitBytes: 1000000, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
                .CreateLogger();

            // Navigation store
            _navigationStore = new NavigationStore();
            
            
            Log.Debug("App created...");
        }

        /// <summary>
        /// Fires when app is starting
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Information("App starting...");

            // Navigation
            var navService = new NavigationService(_navigationStore);
            // Mit login anfangen
            _navigationStore.CurrentViewModel = new LoginViewModel(navService);

            // Der view container erstellen
            MainWindow mainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore)
            };
            mainWindow.Show();

            // Andere Sachen
            base.OnStartup(e);
        }

        /// <summary>
        /// Fires when app is closed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("App closing...");
            var currentViewModel = _navigationStore.CurrentViewModel;
        }
    }
}
