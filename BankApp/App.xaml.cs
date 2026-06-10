using BankApp.ViewModels;
using BankApp.Stores;
using BankApp.Services;
using System.Windows;
using Serilog;

namespace BankApp
{
    public partial class App : Application
    {
        private NavigationStore _navigationStore;

        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Month, fileSizeLimitBytes: 1000000, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
                .CreateLogger();

            _navigationStore = new NavigationStore();
            Log.Debug("App created...");
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            Log.Information("App starting...");

            var services = new AppServices("http://127.0.0.1:8000", _navigationStore);

            var account = await services.AccountService.GetMeAsync();
            if (account != null)
                _navigationStore.CurrentViewModel = new AccountViewModel(services, account);
            else
                _navigationStore.CurrentViewModel = new LoginViewModel(services);

            MainWindow mainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_navigationStore)
            };
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("App closing...");
        }
    }
}
