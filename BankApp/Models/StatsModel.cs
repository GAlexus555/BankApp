using System.Text.Json.Serialization;

namespace BankApp.Models;

public class StatsModel
{
    [JsonPropertyName("account_id")]    public int    AccountId        { get; set; }
    [JsonPropertyName("firstname")]     public string FirstName        { get; set; } = "";
    [JsonPropertyName("lastname")]      public string LastName         { get; set; } = "";
    [JsonPropertyName("transaction_count")] public int TransactionCount { get; set; }
    [JsonPropertyName("total_cents")]   public int    TotalCents       { get; set; }

    public string DisplayName  => $"{FirstName} {LastName}";
    public string DisplayTotal => $"€ {TotalCents / 100.0:F2}";
}
