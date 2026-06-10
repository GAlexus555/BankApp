using BankApp.Models;
using BankApp.Services;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Windows;

namespace BankApp.ViewModels;

public class MyTransactionsViewModel : ViewModelBase
{
    private readonly AppServices _services;
    private readonly AccountModel _account;

    public string ClientName { get; }
    public string ClientEmail { get; }

    private List<TransactionModel> _transactions = new();
    public List<TransactionModel> Transactions
    {
        get => _transactions;
        private set { _transactions = value; OnPropertyChanged(nameof(Transactions)); }
    }

    public IRelayCommand BackCommand { get; }

    public MyTransactionsViewModel(AppServices services, AccountModel account)
    {
        _services = services;
        _account = account;
        ClientName = $"{account.FirstName} {account.LastName}";
        ClientEmail = account.Email;

        BackCommand = new RelayCommand(() =>
            services.NavigationService.Navigate(new AccountViewModel(services, account)));

        LoadTransactions();
    }

    private async void LoadTransactions()
    {
        var result = await _services.TransactionService.GetMyTransactions();
        if (result == null) { MessageBox.Show("Transaktionen konnten nicht geladen werden."); return; }
        Transactions = result;
    }
}
