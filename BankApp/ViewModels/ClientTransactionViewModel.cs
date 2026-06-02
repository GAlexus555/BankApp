using BankApp.Models;
using BankApp.Services;
using BankApp.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;

namespace BankApp.ViewModels;

public class ClientTransactionViewModel : ViewModelBase
{
    private readonly NavigationService _navService;
    private readonly IAccountService _accService;
    private readonly ITransactionService _trService;
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

    public ClientTransactionViewModel(NavigationService navService, AccountModel client, IAccountService accService, ITransactionService trService, HttpClient httpClient)
    {
        _navService = navService;
        _accService = accService;
        _trService = trService;
        _client = httpClient;

        ClientName = $"{client.FirstName} {client.LastName}";
        ClientEmail = client.Email;

        BackCommand = new RelayCommand(() =>
            navService.Navigate(new ManagerViewModel(navService, accService, trService, httpClient)));

        LoadTransactions(client.Id);
    }

    private async void LoadTransactions(int accountId)
    {
        var result = await _trService.GetTransactionsByAccountId(accountId);
        if (result == null)
        {
            MessageBox.Show("Transaktionen konnten nicht geladen werden.");
            return;
        }
        Transactions = result;
    }
}
