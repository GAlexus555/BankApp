using BankApp.Models;
using BankApp.Services;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Windows;

namespace BankApp.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private string _username = "";
    public string Username
    {
        get => _username;
        set { _username = value; OnPropertyChanged(nameof(Username)); }
    }

    private string _password = "";
    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(nameof(Password)); }
    }

    public IAsyncRelayCommand LoginCommand { get; }

    private readonly AppServices _services;

    public LoginViewModel(AppServices services)
    {
        _services = services;
        LoginCommand = new AsyncRelayCommand(LoginAccount);
        Log.Debug("Created login view model");
    }

    private async Task LoginAccount()
    {
        var success = await _services.AccountService.LoginAsync(Username, Password);
        if (!success)
        {
            MessageBox.Show("Login data is wrong, please repeat with valid credentials.");
            return;
        }

        var account = await _services.AccountService.GetMeAsync();
        if (account == null)
        {
            MessageBox.Show("Failed to retrieve account data.");
            return;
        }

        if (account.Role == AccountRole.Manager)
            _services.NavigationService.Navigate(new ManagerViewModel(_services));
        else
            _services.NavigationService.Navigate(new AccountViewModel(_services, account));
    }
}
