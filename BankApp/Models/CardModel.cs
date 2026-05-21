using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Models;

public class CardModel
{
    public string IBAN { get; }
    public string CardNr { get; }
    public DateTime ExpireDate { get; }
    public int CVC { get; }

    private int _pin;

    public CardModel(string iban, string cardNr, DateTime expireDate, int cvc)
    {
        IBAN = iban;
        CardNr = cardNr;
        ExpireDate = expireDate;
        CVC = cvc;
    }
}
