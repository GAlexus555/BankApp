using System;
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
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("cents")]
    public int Cents { get; set; }
    [JsonPropertyName("status")]
    public CardStatus CardStatus { get; set; }
    [JsonPropertyName("owner_id")]
    public int OwnerId { get; set; }
    [JsonPropertyName("iban")]
    public string IBAN { get; set; } = "";
    [JsonPropertyName("card_nr")]
    public string CardNr { get; set; } = "";
    [JsonPropertyName("expire_date")]
    public DateTime ExpireDate { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("cvc")]
    public int CVC { get; set; }

    public string DisplayBalance => $"€ {Cents / 100.0:F2}";
}
