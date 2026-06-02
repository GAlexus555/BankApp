using BankApp.Commands;
using BankApp.Models;
using BankApp.Services;
using BankApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows.Input;

namespace BankApp.ViewModels;

public class CardViewModel : ViewModelBase
{
    private CardModel _card;

    public string Cents
    {
        get
        {
            int eur = _card.Cents / 100;
            int cents = _card.Cents % 100;
            return $"{eur:N0}.{cents:D2} €";
        }
    }
    public string IBAN { get => _card.IBAN; }
    public string CardNr { get => _card.CardNr; }
    public string ExpireDate { get => _card.ExpireDate.ToShortDateString(); }
    public string CVC { get => _card.CVC.ToString(); }

    public ICommand NewTransaction { get; }

    private NavigationService navService;

    public CardViewModel (CardModel card, NavigationService navigation, HttpClient client, AccountModel account, IAccountService accountService)
    {
        navService = navigation;
        _card = card;

        TransactionService transactionService = new TransactionService(client); 
        NewTransaction = new NavigateCommand(navService, () => new TransactionViewModel(this, account, accountService, navService, transactionService, client));
    }
}
