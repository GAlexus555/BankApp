using BankApp.Commands;
using BankApp.Models;
using BankApp.Services;
using BankApp.Views;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BankApp.ViewModels;

public class AccountViewModel : ViewModelBase
{
    private readonly AppServices _services;
    private readonly AccountModel? _account;

    public ICommand BackCommand { get; }
    public IRelayCommand ShowTransactionsCommand { get; }
    public IRelayCommand ShowInterestsCommand { get; }
    public IRelayCommand EditProfileCommand { get; }
    public IRelayCommand LogoutCommand { get; }

    public string Name => _account == null ? "Dummy" : $"{_account.FirstName} {_account.LastName}";

    public ObservableCollection<CardViewModel> Cards { get; set; }

    public AccountViewModel(AppServices services, AccountModel? account)
    {
        _services = services;
        _account = account;

        BackCommand = new NavigateCommand(services.NavigationService, () => new LoginViewModel(services));
        ShowTransactionsCommand = new RelayCommand(() =>
            services.NavigationService.Navigate(new MyTransactionsViewModel(services, account!)));
        ShowInterestsCommand = new RelayCommand(() =>
            services.NavigationService.Navigate(new MyInterestsViewModel(services, account!)));
        EditProfileCommand = new AsyncRelayCommand(async () =>
        {
            if (_account == null) return;
            var dialog = new EditProfileDialog(_account);
            if (dialog.ShowDialog() != true) return;
            bool ok = await services.AccountService.UpdateMeAsync(
                dialog.FirstName, dialog.LastName, dialog.Email,
                dialog.Phone, dialog.Address, dialog.Birthdate, dialog.Password);
            if (!ok) MessageBox.Show("Profil konnte nicht gespeichert werden.");
        });
        LogoutCommand = new RelayCommand(() =>
        {
            services.AccountService.Logout();
            services.NavigationService.Navigate(new LoginViewModel(services));
        });

        Cards = new ObservableCollection<CardViewModel>();
        LoadCardsAsync();
    }

    private async void LoadCardsAsync()
    {
        if (_account == null) return;
        var cards = await _services.CardService.GetCardsAsync();
        if (cards == null) return;

        _account.Cards = cards;
        Cards.Clear();
        foreach (var card in cards)
            Cards.Add(new CardViewModel(_services, card, _account));
    }
}
