using BankApp.Models;
using BankApp.Services;
using BankApp.Views;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace BankApp.ViewModels
{
    public class ManagerViewModel : ViewModelBase
    {
        private readonly AppServices _services;
        private AccountModel _account;
        private List<CardModel> _allCards = new();

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
                AddCardCommand.NotifyCanExecuteChanged();
                UpdateSelectedCards();
            }
        }

        public string Name => _account == null ? "Loading..." : $"{_account.FirstName} {_account.LastName}";
        public List<object> Collection { get; set; }
        public ObservableCollection<CardModel> SelectedCards { get; } = new();
        private List<AccountModel> Clients { get; set; }
        private List<TransactionModel> Transactions { get; set; }

        public IRelayCommand ShowClients { get; }
        public IRelayCommand ShowTransactions { get; }
        public IRelayCommand DeleteClientCommand { get; }
        public IRelayCommand AddClientCommand { get; }
        public IRelayCommand EditClientCommand { get; }
        public IRelayCommand ViewClientTransactionsCommand { get; }
        public IRelayCommand AddCardCommand { get; }
        public IRelayCommand LogoutCommand { get; }

        public ManagerViewModel(AppServices services)
        {
            _services = services;
            LoadAccount();

            ShowClients = new AsyncRelayCommand(LoadClients);
            ShowTransactions = new AsyncRelayCommand(LoadTransactions);
            DeleteClientCommand = new AsyncRelayCommand(DeleteClient, CanSelectClient);
            AddClientCommand = new AsyncRelayCommand(AddClient);
            EditClientCommand = new AsyncRelayCommand(EditClient, CanSelectClient);
            ViewClientTransactionsCommand = new RelayCommand(ViewClientTransactions, CanSelectClient);
            AddCardCommand = new AsyncRelayCommand(AddCard, CanSelectClient);
            LogoutCommand = new RelayCommand(() =>
            {
                _services.AccountService.Logout();
                _services.NavigationService.Navigate(new LoginViewModel(_services));
            });
        }

        private async void LoadAccount()
        {
            _account = await _services.AccountService.GetMeAsync();
            if (_account == null) { MessageBox.Show("Failed to retrieve account data."); return; }
            OnPropertyChanged(nameof(Name));
            await LoadClients();
        }

        private async Task LoadClients()
        {
            Clients = await _services.AccountService.GetAllAccountsAsync() ?? new();

            // List<InterestModel> ii = await _services.InterestsService.GetAllInterestsAsync();

            var cardTasks = Clients.Select(c => _services.CardService.GetCardsByAccountIdAsync(c.Id));
            var results = await Task.WhenAll(cardTasks);

            _allCards = new();
            for (int i = 0; i < Clients.Count; i++)
            {
                var cards = results[i] ?? new();
                Clients[i].Cards = cards;
                _allCards.AddRange(cards);
            }

            Collection = new List<object>(Clients.Cast<object>());
            OnPropertyChanged(nameof(Collection));
            UpdateSelectedCards();
        }

        private async Task LoadTransactions()
        {
            Transactions = await _services.TransactionService.GetAllTransactions();
            if (Transactions == null) { MessageBox.Show("Failed to load transactions."); return; }
            Collection = new List<object>(Transactions.Cast<object>());
            OnPropertyChanged(nameof(Collection));
        }

        private void UpdateSelectedCards()
        {
            SelectedCards.Clear();
            if (SelectedItem is AccountModel client)
            {
                foreach (var card in _allCards.Where(c => c.OwnerId == client.Id))
                    SelectedCards.Add(card);
            }
        }

        private bool CanSelectClient() => SelectedItem is AccountModel;

        private void ViewClientTransactions()
        {
            if (SelectedItem is not AccountModel client) return;
            _services.NavigationService.Navigate(new ClientTransactionViewModel(_services, client));
        }

        private async Task DeleteClient()
        {
            if (SelectedItem is not AccountModel client) return;
            var confirm = MessageBox.Show(
                $"Konto von {client.FirstName} {client.LastName} wirklich löschen?",
                "Bestätigung", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            bool ok = await _services.AccountService.DeleteAccountAsync(client.Id);
            if (!ok) { MessageBox.Show("Löschen fehlgeschlagen."); return; }
            await LoadClients();
        }

        private async Task EditClient()
        {
            if (SelectedItem is not AccountModel client) return;
            var dialog = new EditClientDialog(client);
            if (dialog.ShowDialog() != true) return;

            bool ok = await _services.AccountService.UpdateAccountAsync(
                client.Id, dialog.FirstName, dialog.LastName,
                dialog.Email, dialog.Phone, dialog.Address);
            if (!ok) { MessageBox.Show("Bearbeiten fehlgeschlagen."); return; }
            await LoadClients();
        }

        private async Task AddClient()
        {
            var dialog = new AddClientDialog();
            if (dialog.ShowDialog() != true) return;

            var result = await _services.AccountService.CreateAccountAsync(
                dialog.FirstName, dialog.LastName, dialog.Email,
                dialog.Password, dialog.Phone, dialog.Address, dialog.Birthdate);
            if (result == null) { MessageBox.Show("Kunde konnte nicht erstellt werden."); return; }
            await LoadClients();
        }

        private async Task AddCard()
        {
            if (SelectedItem is not AccountModel client) return;
            var dialog = new AddCardDialog();
            if (dialog.ShowDialog() != true) return;

            var result = await _services.CardService.CreateCardAsync(
                dialog.InitialCents, CardStatus.Inactive, client.Id,
                dialog.IBAN, dialog.CardNr, dialog.CVC, dialog.ExpireDate);
            if (result == null) { MessageBox.Show("Karte konnte nicht erstellt werden."); return; }
            await LoadClients();
        }
    }
}
