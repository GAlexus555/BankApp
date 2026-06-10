using BankApp.Commands;
using BankApp.Models;
using BankApp.Services;
using BankApp.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows;
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
    public IAsyncRelayCommand LoginCommand { get; }

    //Navigation
    private NavigationService _navigationService;
    // API
    private IAccountService _accountService;
    private ITransactionService _transactionService;
    private ICardService _cardService;
    private HttpClient _client;
    // Account
    private AccountModel? _account;

    public LoginViewModel(NavigationService navigationService, HttpClient client)
    {
        _navigationService = navigationService;
        _accountService = new AccountService(client);
        _transactionService = new TransactionService(client);
        _cardService = new CardService(client);
        _client = client;

        LoginCommand = new AsyncRelayCommand(LoginAccount);

        Log.Debug("Created login view model");
    }

    private async Task LoginAccount()
    {
        var success = await _accountService.LoginAsync(Username, Password);
        if (!success)
        {
            MessageBox.Show("Login data is wrong, please repeat with valid credentials.");
            return;
        }

        var account = await _accountService.GetMeAsync();
        if (account == null)
        {
            MessageBox.Show("Failed to retrieve account data.");
            return;
        }

        account.Cards = await _accountService.GetCardsAsync(account.Id);

        if (account.Role == AccountRole.Manager)
        {
            _navigationService.Navigate(new ManagerViewModel(_navigationService, _accountService, _transactionService, _cardService, _client));
        }
        else
        {
            _navigationService.Navigate(new AccountViewModel(_navigationService, account, _client, _accountService));
        }
    }
}
