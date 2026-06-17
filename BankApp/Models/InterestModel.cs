using System;
using System.Text.Json.Serialization;

namespace BankApp.Models;

public class InterestModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("card_id")]
    public int CardId { get; set; }
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
    [JsonPropertyName("created_at")]
    public DateOnly CreatedAt { get; set; }
    [JsonPropertyName("withdrawn")]
    public bool Withdrawn { get; set; }
    [JsonPropertyName("interest_rate")]
    public double InterestRate { get; set; }

    public double CurrentAmount
    {
        get
        {
            double years = (DateTime.Today - CreatedAt.ToDateTime(TimeOnly.MinValue)).Days / 365.25;
            return Amount * Math.Pow(1 + InterestRate, years);
        }
    }

    public string DisplayInvested     => $"€ {Amount / 100.0:F2}";
    public string DisplayCurrentValue => $"€ {CurrentAmount / 100.0:F2}";
    public string DisplayGrowth       => $"+€ {Math.Max(0, CurrentAmount - Amount) / 100.0:F2}";
    public string DisplayRate         => $"{InterestRate * 100:F2} %";
    public string DisplayDate         => CreatedAt.ToString("dd.MM.yyyy");
    public string? CardIBAN            { get; set; }
    public string DisplayCardId       => CardIBAN ?? $"Karte #{CardId}";
    public string StatusText          => Withdrawn ? "Ausgezahlt" : "● Aktiv";
    public bool   IsActive            => !Withdrawn;
}
