using BankApp.Models;
using BankApp.Services;
using BankApp.Views;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace BankApp.ViewModels
{
    public enum ManagerActiveView { Clients, Transactions, Stats, AuditLogs }

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
        public ObservableCollection<CardModel>    SelectedCards { get; } = new();
        public ObservableCollection<StatsModel>   Stats         { get; } = new();
        public ObservableCollection<AuditLogModel> AuditLogs    { get; } = new();

        public ISeries[] TxCountSeries { get; private set; } = [];
        public ISeries[] VolumeSeries  { get; private set; } = [];
        public Axis[]    StatsXAxes    { get; private set; } = [];

        private List<AccountModel>     Clients      { get; set; }
        private List<TransactionModel> Transactions { get; set; }

        private ManagerActiveView _activeView = ManagerActiveView.Clients;
        public ManagerActiveView ActiveView
        {
            get => _activeView;
            private set
            {
                _activeView = value;
                OnPropertyChanged(nameof(ActiveView));
                OnPropertyChanged(nameof(IsClientView));
                OnPropertyChanged(nameof(IsTransactionView));
                OnPropertyChanged(nameof(IsStatsView));
                OnPropertyChanged(nameof(IsAuditView));
            }
        }

        public bool IsClientView      => ActiveView == ManagerActiveView.Clients;
        public bool IsTransactionView => ActiveView == ManagerActiveView.Transactions;
        public bool IsStatsView       => ActiveView == ManagerActiveView.Stats;
        public bool IsAuditView       => ActiveView == ManagerActiveView.AuditLogs;

        private string _transactionTitle = "Alle Überweisungen";
        public string TransactionTitle
        {
            get => _transactionTitle;
            private set { _transactionTitle = value; OnPropertyChanged(nameof(TransactionTitle)); }
        }

        public IRelayCommand ShowClients                  { get; }
        public IRelayCommand ShowTransactions             { get; }
        public IRelayCommand ShowInterests                { get; }
        public IRelayCommand ShowStats                    { get; }
        public IRelayCommand ShowAuditLogs                { get; }
        public IRelayCommand DeleteClientCommand          { get; }
        public IRelayCommand AddClientCommand             { get; }
        public IRelayCommand EditClientCommand            { get; }
        public IRelayCommand ViewClientTransactionsCommand{ get; }
        public IRelayCommand AddCardCommand               { get; }
        public IRelayCommand<CardModel> DeleteCardCommand { get; }
        public IRelayCommand LogoutCommand                { get; }

        public ManagerViewModel(AppServices services)
        {
            _services = services;
            LoadAccount();

            ShowClients       = new AsyncRelayCommand(LoadClients);
            ShowTransactions  = new AsyncRelayCommand(LoadTransactions);
            ShowInterests     = new RelayCommand(() =>
                services.NavigationService.Navigate(new ManagerInterestsViewModel(services)));
            ShowStats         = new AsyncRelayCommand(LoadStats);
            ShowAuditLogs     = new AsyncRelayCommand(LoadAuditLogs);
            DeleteCardCommand = new AsyncRelayCommand<CardModel>(DeleteCard);
            DeleteClientCommand           = new AsyncRelayCommand(DeleteClient, CanSelectClient);
            AddClientCommand              = new AsyncRelayCommand(AddClient);
            EditClientCommand             = new AsyncRelayCommand(EditClient, CanSelectClient);
            ViewClientTransactionsCommand = new AsyncRelayCommand(ViewClientTransactions, CanSelectClient);
            AddCardCommand                = new AsyncRelayCommand(AddCard, CanSelectClient);
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

        // prompt: Lade alle Clients parallel mit Task.WhenAll damit Karten aller Accounts gleichzeitig abgerufen werden statt nacheinander
        private async Task LoadClients()
        {
            ActiveView = ManagerActiveView.Clients;
            Clients = await _services.AccountService.GetAllAccountsAsync() ?? new();

            var cardTasks = Clients.Select(c => _services.CardService.GetCardsByAccountIdAsync(c.Id));
            var results   = await Task.WhenAll(cardTasks);

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
            ActiveView = ManagerActiveView.Transactions;
            TransactionTitle = "Alle Überweisungen";
            Transactions = await _services.TransactionService.GetAllTransactions();
            if (Transactions == null) { MessageBox.Show("Transaktionen konnten nicht geladen werden."); return; }
            Collection = new List<object>(Transactions.Cast<object>());
            OnPropertyChanged(nameof(Collection));
        }

        // prompt: Baue LiveCharts2 ColumnSeries für Transaktionsanzahl und Volumen aus den Stats-Daten auf, sortiert nach Transaktionsanzahl absteigend
        private async Task LoadStats()
        {
            ActiveView = ManagerActiveView.Stats;
            Stats.Clear();
            var result = await _services.StatsService.GetTransactionStatsAsync();
            if (result == null) { MessageBox.Show("Statistiken konnten nicht geladen werden."); return; }

            var ordered = result.OrderByDescending(x => x.TransactionCount).ToList();
            foreach (var s in ordered) Stats.Add(s);

            var labels = ordered.Select(s => s.DisplayName).ToArray();

            TxCountSeries = [new ColumnSeries<int>
            {
                Name        = "Transaktionen",
                Values      = ordered.Select(s => s.TransactionCount).ToArray(),
                Fill        = new SolidColorPaint(new SKColor(26, 86, 219)),
                Rx          = 4, Ry = 4,
                MaxBarWidth = 40,
            }];

            VolumeSeries = [new ColumnSeries<double>
            {
                Name        = "Gesamtvolumen (€)",
                Values      = ordered.Select(s => s.TotalCents / 100.0).ToArray(),
                Fill        = new SolidColorPaint(new SKColor(5, 150, 105)),
                Rx          = 4, Ry = 4,
                MaxBarWidth = 40,
            }];

            StatsXAxes = [new Axis
            {
                Labels         = labels,
                LabelsRotation = -25,
                TextSize       = 11,
            }];

            OnPropertyChanged(nameof(TxCountSeries));
            OnPropertyChanged(nameof(VolumeSeries));
            OnPropertyChanged(nameof(StatsXAxes));
        }

        private async Task LoadAuditLogs()
        {
            ActiveView = ManagerActiveView.AuditLogs;
            AuditLogs.Clear();
            var result = await _services.StatsService.GetAuditLogsAsync();
            if (result == null) { MessageBox.Show("Audit-Log konnte nicht geladen werden."); return; }
            foreach (var l in result) AuditLogs.Add(l);
        }

        private void UpdateSelectedCards()
        {
            SelectedCards.Clear();
            if (SelectedItem is AccountModel client)
                foreach (var card in _allCards.Where(c => c.OwnerId == client.Id))
                    SelectedCards.Add(card);
        }

        private bool CanSelectClient() => SelectedItem is AccountModel;

        private async Task ViewClientTransactions()
        {
            if (SelectedItem is not AccountModel client) return;
            ActiveView = ManagerActiveView.Transactions;
            TransactionTitle = $"{client.FirstName} {client.LastName}  ·  Überweisungen";
            var transactions = await _services.TransactionService.GetTransactionsByAccountId(client.Id);
            if (transactions == null) { MessageBox.Show("Transaktionen konnten nicht geladen werden."); return; }
            Collection = new List<object>(transactions.Cast<object>());
            OnPropertyChanged(nameof(Collection));
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
                dialog.Email, dialog.Phone, dialog.Address,
                dialog.Birthdate, dialog.Password, (int)client.Role);
            if (!ok) { MessageBox.Show("Bearbeiten fehlgeschlagen."); return; }
            await LoadClients();
        }

        private async Task AddClient()
        {
            var dialog = new AddClientDialog();
            if (dialog.ShowDialog() != true) return;

            var result = await _services.AccountService.CreateAccountAsync(
                dialog.FirstName, dialog.LastName, dialog.Email,
                dialog.Password, dialog.Phone, dialog.Address, dialog.Birthdate, dialog.SelectedRole);
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

        private async Task DeleteCard(CardModel? card)
        {
            if (card == null) return;
            var confirm = MessageBox.Show(
                $"Karte {card.IBAN} wirklich löschen?",
                "Bestätigung", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes) return;

            bool ok = await _services.CardService.DeleteCardAsync(card.Id);
            if (!ok) { MessageBox.Show("Karte konnte nicht gelöscht werden."); return; }
            await LoadClients();
        }
    }
}
