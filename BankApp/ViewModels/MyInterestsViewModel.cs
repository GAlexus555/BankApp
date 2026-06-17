using BankApp.Models;
using BankApp.Services;
using BankApp.Views;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace BankApp.ViewModels;

public class MyInterestsViewModel : ViewModelBase
{
    private readonly AppServices _services;
    private readonly AccountModel _account;

    private BankModel? _bank;
    public BankModel? Bank
    {
        get => _bank;
        private set { _bank = value; OnPropertyChanged(nameof(Bank)); OnPropertyChanged(nameof(BankHeader)); }
    }

    public string BankHeader => _bank == null
        ? "Sparzinsen"
        : $"{_bank.BankName}  ·  {_bank.DisplayRate} p.a.";

    public ObservableCollection<InterestModel> Interests { get; } = new();

    public IRelayCommand BackCommand { get; }
    public IRelayCommand CreateCommand { get; }
    public IRelayCommand<InterestModel> WithdrawCommand { get; }

    public MyInterestsViewModel(AppServices services, AccountModel account)
    {
        _services = services;
        _account  = account;

        BackCommand    = new RelayCommand(() => services.NavigationService.Navigate(new AccountViewModel(services, account)));
        CreateCommand  = new AsyncRelayCommand(CreateInterest);
        WithdrawCommand = new AsyncRelayCommand<InterestModel>(Withdraw);

        LoadAsync();
    }

    private async void LoadAsync()
    {
        var bankTask      = _services.BankService.GetBankAsync();
        var interestTask  = _services.InterestsService.GetMyInterestsAsync();
        await Task.WhenAll(bankTask, interestTask);

        Bank = await bankTask;

        var list = await interestTask ?? new();
        Interests.Clear();
        foreach (var i in list.OrderByDescending(x => x.CreatedAt))
            Interests.Add(i);
    }

    private async Task CreateInterest()
    {
        var cards = _account.Cards;
        if (cards == null || cards.Count == 0)
        {
            MessageBox.Show("Keine Karten verfügbar. Bitte zuerst eine Karte anlegen.",
                            "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialog = new CreateInterestDialog(cards, Bank);
        if (dialog.ShowDialog() != true) return;

        var result = await _services.InterestsService.CreateInterestAsync(dialog.SelectedCardId, dialog.AmountCents);
        if (result == null)
        {
            MessageBox.Show("Sparguthaben konnte nicht angelegt werden.", "Fehler",
                            MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Interests.Insert(0, result);
    }

    private async Task Withdraw(InterestModel? interest)
    {
        if (interest == null) return;

        var result = await _services.InterestsService.WithdrawInterestAsync(interest.Id);
        if (result == null)
        {
            MessageBox.Show("Auszahlung fehlgeschlagen.", "Fehler",
                            MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var idx = Interests.IndexOf(interest);
        Interests.Remove(interest);
        Interests.Insert(idx >= 0 ? idx : 0, result);
    }
}
