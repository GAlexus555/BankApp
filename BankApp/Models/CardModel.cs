using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BankApp.Models;

public enum CardStatus
{
    Active,
    Inactive,
    Blocked,
    Expired
}

public class CardModel
{
    [JsonPropertyName("cents")]
    public int Cents { get; }
    [JsonPropertyName("status")]
    public CardStatus CardStatus { get; }
    [JsonPropertyName("iban")]
    public string IBAN { get; }
    [JsonPropertyName("card_nr")]
    public string CardNr { get; }
    [JsonPropertyName("expire_date")]
    public DateTime ExpireDate { get; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; }
    [JsonPropertyName("cvc")]
    public int CVC { get; }

    private int _pin;

    public CardModel(int cents, string iban, string cardNr, DateTime expireDate, int cvc)
    {
        Cents = cents;
        IBAN = iban;
        CardNr = cardNr;
        ExpireDate = expireDate;
        CVC = cvc;
    }
}
