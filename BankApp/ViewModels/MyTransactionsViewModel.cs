using BankApp.Models;
using BankApp.Services;
using BankApp.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;

namespace BankApp.ViewModels;

public class MyTransactionsViewModel : ViewModelBase
{
    private readonly NavigationService _navService;
    private readonly AccountModel _account;
    private readonly IAccountService _accService;
    private readonly HttpClient _client;

    public string ClientName { get; }
    public string ClientEmail { get; }

    private List<TransactionModel> _transactions = new();
    public List<TransactionModel> Transactions
    {
        get => _transactions;
        private set { _transactions = value; OnPropertyChanged(nameof(Transactions)); }
    }

    public IRelayCommand BackCommand { get; }

    public MyTransactionsViewModel(NavigationService navService, AccountModel account, IAccountService accService, ITransactionService trService, HttpClient client)
    {
        _navService = navService;
        _account = account;
        _accService = accService;
        _client = client;

        ClientName = $"{account.FirstName} {account.LastName}";
        ClientEmail = account.Email;

        BackCommand = new RelayCommand(() =>
            navService.Navigate(new AccountViewModel(navService, account, client, accService)));

        LoadTransactions(trService);
    }

    private async void LoadTransactions(ITransactionService trService)
    {
        var result = await trService.GetMyTransactions();
        if (result == null)
        {
            MessageBox.Show("Transaktionen konnten nicht geladen werden.");
            return;
        }
        Transactions = result;
    }
}
