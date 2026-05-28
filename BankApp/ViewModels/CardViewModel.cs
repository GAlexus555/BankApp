using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BankApp.ViewModels;

public class CardViewModel : ViewModelBase
{
    private CardModel _card;

    public string IBAN { get => _card.IBAN; }
    public string CardNr { get => _card.CardNr; }
    public string ExpireDate { get => _card.ExpireDate.ToShortDateString(); }
    public string CVC { get => _card.CVC.ToString(); }

    public CardViewModel (CardModel card)
    {
        _card = card;
    }
}
