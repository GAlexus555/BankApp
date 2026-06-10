using BankApp.Commands;
using BankApp.Models;
using BankApp.Services;
using System.Windows.Input;
using System.Windows.Media;

namespace BankApp.ViewModels;

public class CardViewModel : ViewModelBase
{
    private readonly CardModel _card;

    public string Cents
    {
        get
        {
            int eur   = _card.Cents / 100;
            int cents = _card.Cents % 100;
            return $"{eur:N0}.{cents:D2} €";
        }
    }

    public string IBAN       => _card.IBAN;
    public string CardNr     => _card.CardNr;
    public string ExpireDate => _card.ExpireDate.ToString("MM/yy");
    public string CVC        => _card.CVC.ToString();

    public string CardStatusText => _card.CardStatus switch
    {
        CardStatus.Active   => "● Aktiv",
        CardStatus.Blocked  => "● Gesperrt",
        CardStatus.Expired  => "● Abgelaufen",
        _                   => "● Inaktiv"
    };

    public Brush CardStatusBrush => _card.CardStatus switch
    {
        CardStatus.Active  => new SolidColorBrush(Color.FromRgb(0x4A, 0xDE, 0x80)),
        CardStatus.Blocked => new SolidColorBrush(Color.FromRgb(0xF8, 0x71, 0x71)),
        CardStatus.Expired => new SolidColorBrush(Color.FromRgb(0xFB, 0xBF, 0x24)),
        _                  => new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8))
    };

    public ICommand NewTransaction { get; }

    public CardViewModel(AppServices services, CardModel card, AccountModel account)
    {
        _card = card;
        NewTransaction = new NavigateCommand(services.NavigationService,
            () => new TransactionViewModel(services, this, account));
    }
}
