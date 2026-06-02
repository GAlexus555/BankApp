using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BankApp.Models;

public enum TransactionStatus
{
    Pending,
    Sent
}

public class TransactionModel
{
    [JsonPropertyName("id")]
    public int ID { get; set;  }
    [JsonPropertyName("iban_from")]
    public string IBANFrom { get; set; }
    [JsonPropertyName("iban_to")]
    public string IBANTo { get; set; }
    [JsonPropertyName("amount_cents")]
    public double AmountCents { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("status")]
    public TransactionStatus Status { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }

    public double AmountEuros => AmountCents / 100.0;

    public override string ToString() => $"{CreatedAt:dd.MM.yyyy}  |  € {AmountCents / 100.0:F2}  |  {IBANFrom}  →  {IBANTo}  |  \"{Description}\"  [{Status}]";
}
