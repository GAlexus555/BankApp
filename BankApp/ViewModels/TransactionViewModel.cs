using BankApp.Services;
using BankApp.Models;
using BankApp.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using BankApp.Commands;
using System.Windows;

namespace BankApp.ViewModels;

public class TransactionViewModel : ViewModelBase
{
    private CardViewModel _card;
    private NavigationService _navService;
    private ITransactionService _service;
    private AccountModel _account;
    private IAccountService _accountService;
    private HttpClient _client;

    public string IbanFrom { get => _card.IBAN; }
    public string IbanTo { get; set; }
    public string Description { get; set; }
    public int Amount { get; set; }

    public IRelayCommand NewTransaction { get; }
    public ICommand Back { get; }

    public TransactionViewModel(CardViewModel card, AccountModel account, IAccountService accountService, NavigationService navService, ITransactionService service, HttpClient client)
    {
        _card = card;
        _account = account;
        _accountService = accountService;
        _navService = navService;
        _service = service;
        _client = client;

        Back = new NavigateCommand(_navService, () => new AccountViewModel(_navService, _account, _client, _accountService));
        NewTransaction = new AsyncRelayCommand(CreateTransaction);
    }

    private async Task CreateTransaction()
    {
        var success = await _service.CreateNewTransaction(IbanFrom, IbanTo, Amount, Description, TransactionStatus.Pending);
        if (success == null)
        {
            MessageBox.Show("Transaction couldn't be created, input data is wrong!");
            return;
        }

        var account = await _accountService.GetMeAsync();
        if (account == null)
        {
            MessageBox.Show("Failed to retrieve account data.");
            return;
        }

        account.Cards = await _accountService.GetCardsAsync(account.Id);

        MessageBox.Show("Transaction has been succeeded");
        _navService.Navigate(new AccountViewModel(_navService, account, _client, _accountService));
    }
}
