using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BankApp.Models;

public enum AccountRole
{
    Client, 
    Manager
}

public class AccountModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("firstname")]
    public string FirstName { get; set; } = "";

    [JsonPropertyName("lastname")]
    public string LastName { get; set; } = "";

    [JsonPropertyName("role")]
    public AccountRole Role { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = "";

    [JsonPropertyName("phonenumber")]
    public string Phone { get; set; } = "";

    [JsonPropertyName("address")]
    public string Address { get; set; } = "";

    [JsonPropertyName("birthdate")]
    public DateTime Birthdate { get; set; }

    [JsonPropertyName("createdat")]
    public DateTime CreatedAt { get; set; }

    public List<CardModel>? Cards { get; set; }
}
