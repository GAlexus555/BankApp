using BankApp.Models;
using BankApp.Services;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;

namespace BankApp.ViewModels;

public class ManagerInterestsViewModel : ViewModelBase
{
    private readonly AppServices _services;

    public ObservableCollection<InterestModel> Interests { get; } = new();

    public IRelayCommand BackCommand { get; }

    public ManagerInterestsViewModel(AppServices services)
    {
        _services = services;
        BackCommand = new RelayCommand(() => services.NavigationService.Navigate(new ManagerViewModel(services)));
        LoadAsync();
    }

    private async void LoadAsync()
    {
        var interestTask = _services.InterestsService.GetAllInterestsAsync();
        var cardTask     = _services.CardService.GetAllCardsAsync();
        await Task.WhenAll(interestTask, cardTask);

        var cards = (await cardTask ?? new()).ToDictionary(c => c.Id, c => c.IBAN);
        var list  = await interestTask ?? new();

        Interests.Clear();
        foreach (var i in list.OrderByDescending(x => x.CreatedAt))
        {
            if (cards.TryGetValue(i.CardId, out var iban))
                i.CardIBAN = iban;
            Interests.Add(i);
        }
    }
}
