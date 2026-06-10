using BankApp.Models;
using BankApp.Services;
using BankApp.Services.Interfaces;
using BankApp.Views;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace BankApp.ViewModels
{
    public class ManagerViewModel : ViewModelBase
    {
        private AccountModel account;
        private NavigationService _navService;
        private IAccountService _accService;
        private ITransactionService _trService;
        private ICardService _cardService;
        private HttpClient _client;

        private object? _selectedItem;
        public object? SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                DeleteClientCommand.NotifyCanExecuteChanged();
                EditClientCommand.NotifyCanExecuteChanged();
                ViewClientTransactionsCommand.NotifyCanExecuteChanged();
            }
        }

        public string Name { get => account == null ? "Loading..." : $"{account.FirstName} {account.LastName}"; }
        public List<object> Collection { get; set; }
        private List<AccountModel> Clients { get; set; }
        private List<TransactionModel> Transactions { get; set; }

        public IRelayCommand ShowClients { get; }
        public IRelayCommand ShowTransactions { get; }
        public IRelayCommand DeleteClientCommand { get; }
        public IRelayCommand AddClientCommand { get; }
        public IRelayCommand EditClientCommand { get; }
        public IRelayCommand ViewClientTransactionsCommand { get; }

        public ManagerViewModel(NavigationService navigationService, IAccountService accountService, ITransactionService transactionService, ICardService cardSevice, HttpClient client)
        {
            _navService = navigationService;
            _accService = accountService;
            _trService = transactionService;
            _cardService = cardSevice;
            _client = client;
            LoadAccount();

            ShowClients = new AsyncRelayCommand(LoadClients);
            ShowTransactions = new AsyncRelayCommand(LoadTransactions);
            DeleteClientCommand = new AsyncRelayCommand(DeleteClient, CanDeleteClient);
            AddClientCommand = new AsyncRelayCommand(AddClient);
            EditClientCommand = new AsyncRelayCommand(EditClient, CanDeleteClient);
            ViewClientTransactionsCommand = new RelayCommand(ViewClientTransactions, CanDeleteClient);
        }

        private async void LoadAccount()
        {
            account = await _accService.GetMeAsync();
            if (account == null)
            {
                MessageBox.Show("Failed to retrieve account data.");
                return;
            }

            OnPropertyChanged(nameof(Name));
            LoadClients();
        }

        private async Task LoadClients()
        {
            Clients = await _accService.GetAllAccountsAsync();
            Collection = new List<object>();
            foreach (var client in Clients)
            {
                Collection.Add(client);
            }
            OnPropertyChanged(nameof(Collection));
        }

        private async Task LoadTransactions()
        {
            Transactions = await _trService.GetAllTransactions();
            if (Transactions == null)
            {
                MessageBox.Show("Failed to load transactions.");
                return;
            }
            Collection = new List<object>();
            foreach (var transaction in Transactions)
            {
                Collection.Add(transaction);
            }
            OnPropertyChanged(nameof(Collection));
        }

        private bool CanDeleteClient() => SelectedItem is AccountModel;

        private void ViewClientTransactions()
        {
            if (SelectedItem is not AccountModel client) return;
            _navService.Navigate(new ClientTransactionViewModel(_navService, client, _accService, _trService, _cardService, _client));
        }

        private async Task DeleteClient()
        {
            if (SelectedItem is not AccountModel client) return;

            var confirm = MessageBox.Show(
                $"Konto von {client.FirstName} {client.LastName} wirklich löschen?",
                "Bestätigung", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            bool ok = await _accService.DeleteAccountAsync(client.Id);
            if (!ok) { MessageBox.Show("Löschen fehlgeschlagen."); return; }

            await LoadClients();
        }

        private async Task EditClient()
        {
            if (SelectedItem is not AccountModel client) return;

            var dialog = new EditClientDialog(client);
            if (dialog.ShowDialog() != true) return;

            bool ok = await _accService.UpdateAccountAsync(
                client.Id,
                dialog.FirstName, dialog.LastName,
                dialog.Email, dialog.Phone, dialog.Address);

            if (!ok) { MessageBox.Show("Bearbeiten fehlgeschlagen."); return; }

            await LoadClients();
        }

        private async Task AddClient()
        {
            var dialog = new AddClientDialog();
            if (dialog.ShowDialog() != true) return;

            var result = await _accService.CreateAccountAsync(
                dialog.FirstName, dialog.LastName, dialog.Email,
                dialog.Password, dialog.Phone, dialog.Address, dialog.Birthdate);

            if (result == null) { MessageBox.Show("Kunde konnte nicht erstellt werden."); return; }

            await LoadClients();
        }
    }
}