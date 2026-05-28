using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using BankApp.Commands;
using BankApp.Models;
using BankApp.Services;

namespace BankApp.ViewModels;

public class AccountViewModel : ViewModelBase
{
    //Navigation
    private NavigationService _navigationService;

    public ICommand BackCommand { get; }

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

    public AccountViewModel(NavigationService navigationService, AccountModel? account, HttpClient client)
    {
        _navigationService = navigationService;
        BackCommand = new NavigateCommand(_navigationService, () => new LoginViewModel(navigationService, client));
        _account = account;

        // Fill cards
        Cards = new ObservableCollection<CardViewModel>();

        if (_account != null && _account.Cards != null)
        {
            foreach (var card in _account.Cards)
            {
                Cards.Add(new CardViewModel(card));
            }
        }
    }
}
