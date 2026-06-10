using BankApp.Commands;
using BankApp.Models;
using BankApp.Services;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BankApp.ViewModels;

public class AccountViewModel : ViewModelBase
{
    private readonly AppServices _services;
    private readonly AccountModel? _account;

    public ICommand BackCommand { get; }
    public IRelayCommand ShowTransactionsCommand { get; }
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
        LogoutCommand = new RelayCommand(() =>
        {
            services.AccountService.Logout();
            services.NavigationService.Navigate(new LoginViewModel(services));
        });

        Cards = new ObservableCollection<CardViewModel>();
        if (_account?.Cards != null)
        {
            foreach (var card in _account.Cards)
                Cards.Add(new CardViewModel(services, card, _account));
        }
    }
}
