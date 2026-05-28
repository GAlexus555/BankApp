using BankApp.ViewModels;
using BankApp.Stores;
using BankApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using Serilog;
using System.Net.Http;

namespace BankApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private NavigationStore _navigationStore;
        private Uri _apiAdress;

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

            // Address
            _apiAdress = new Uri("http://127.0.0.1:8000");
            
            
            Log.Debug("App created...");
        }

        /// <summary>
        /// Fires when app is starting
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // To DO: implement service collection and singletons and transients

            Log.Information("App starting...");

            // Navigation
            var navService = new NavigationService(_navigationStore);

            // Client, für Backend Verbindung
            HttpClient client = new HttpClient()
            {
                BaseAddress = _apiAdress,
            };

            // Mit login anfangen
            _navigationStore.CurrentViewModel = new LoginViewModel(navService, client);

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
