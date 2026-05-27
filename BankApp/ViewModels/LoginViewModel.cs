using BankApp.Commands;
using BankApp.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace BankApp.ViewModels;

public class LoginViewModel : ViewModelBase
{
    //Properties
    private string _username;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    private string _password;
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    //Commands
    public ICommand LoginCommand { get; }

    //Navigation
    private NavigationService _navigationService;


    public LoginViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
        LoginCommand = new NavigateCommand(navigationService, () => new AccountViewModel(navigationService));

        Log.Debug("Created login view model");
    }
}
