using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using BankApp.Commands;
using BankApp.Models;
using BankApp.Services;
using BankApp.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;

namespace BankApp.ViewModels;

public class AccountViewModel : ViewModelBase
{
    //Navigation
    private NavigationService _navigationService;

    public ICommand BackCommand { get; }
    public IRelayCommand ShowTransactionsCommand { get; }

    // Model
    private AccountModel? _account { get; set; }

    public string Name
    {
        get
        {
            if (_account == null)
            {
                return "Dummy";
            }
            else
            {
                return $"{_account.FirstName} {_account.LastName}";
            }
        }
    }

    public ObservableCollection<CardViewModel> Cards { get; set; }

    public AccountViewModel(NavigationService navigationService, AccountModel? account, HttpClient client, IAccountService accountService)
    {
        _navigationService = navigationService;
        BackCommand = new NavigateCommand(_navigationService, () => new LoginViewModel(navigationService, client));
        _account = account;

        var trService = new TransactionService(client);
        ShowTransactionsCommand = new RelayCommand(() =>
            navigationService.Navigate(new MyTransactionsViewModel(navigationService, account!, accountService, trService, client)));

        // Fill cards
        Cards = new ObservableCollection<CardViewModel>();

        if (_account != null && _account.Cards != null)
        {
            foreach (var card in _account.Cards)
            {
                Cards.Add(new CardViewModel(card, navigationService, client, _account, accountService));
            }
        }
    }
}
