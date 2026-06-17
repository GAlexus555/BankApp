using System;
using System.Text.Json.Serialization;

namespace BankApp.Models;

public class AuditLogModel
{
    [JsonPropertyName("id")]         public int      Id        { get; set; }
    [JsonPropertyName("account_id")] public int      AccountId { get; set; }
    [JsonPropertyName("timestamp")]  public DateTime Timestamp { get; set; }
    [JsonPropertyName("tablename")]  public string   TableName { get; set; } = "";
    [JsonPropertyName("action")]     public string   Action    { get; set; } = "";
    [JsonPropertyName("details")]    public string?  Details   { get; set; }

    public string DisplayTimestamp => Timestamp.ToString("dd.MM.yyyy HH:mm");
    public string DisplayDetails   => Details ?? "–";
}
