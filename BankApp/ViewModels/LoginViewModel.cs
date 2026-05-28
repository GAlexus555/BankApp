using BankApp.Commands;
using BankApp.Models;
using BankApp.Services;
using BankApp.Services.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Input;

namespace BankApp.ViewModels;

public class LoginViewModel : ViewModelBase
{
    //Properties
    private string _username = "";
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    private string _password = "";
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
    // API
    private IAccountService _accountService;
    // Account
    private AccountModel? _account;

    public LoginViewModel(NavigationService navigationService, HttpClient client)
    {
        _navigationService = navigationService;
        _accountService = new AccountService(client);

        LoadAccount(_accountService);

        LoginCommand = new NavigateCommand(_navigationService, () => new AccountViewModel(_navigationService, _account, client));

        Log.Debug("Created login view model");
    }

    private async void LoadAccount(IAccountService service)
    {
        await service.LoginAsync("alexei.galaburda@gmail.com", "password");
        _account = await service.GetMeAsync();
    }
}
