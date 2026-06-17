using System.Text.Json.Serialization;

namespace BankApp.Models;

public class BankModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("bankname")]
    public string BankName { get; set; } = "";
    [JsonPropertyName("interest_rate")]
    public double InterestRate { get; set; }

    public string DisplayRate => $"{InterestRate * 100:F2} %";
}
