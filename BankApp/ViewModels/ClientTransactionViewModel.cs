using BankApp.Models;
using BankApp.Services;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Windows;

namespace BankApp.ViewModels;

public class ClientTransactionViewModel : ViewModelBase
{
    private readonly AppServices _services;

    public string ClientName { get; }
    public string ClientEmail { get; }

    private List<TransactionModel> _transactions = new();
    public List<TransactionModel> Transactions
    {
        get => _transactions;
        private set { _transactions = value; OnPropertyChanged(nameof(Transactions)); }
    }

    public IRelayCommand BackCommand { get; }

    public ClientTransactionViewModel(AppServices services, AccountModel client)
    {
        _services = services;
        ClientName = $"{client.FirstName} {client.LastName}";
        ClientEmail = client.Email;

        BackCommand = new RelayCommand(() => services.NavigationService.Navigate(new ManagerViewModel(services)));
        LoadTransactions(client.Id);
    }

    private async void LoadTransactions(int accountId)
    {
        var result = await _services.TransactionService.GetTransactionsByAccountId(accountId);
        if (result == null) { MessageBox.Show("Transaktionen konnten nicht geladen werden."); return; }
        Transactions = result;
    }
}
