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

<<<<<<< HEAD
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
=======
    public AccountModel(Guid id, AccountRole role, string email, string phone, string address, DateTime birthDate, List<CardModel>? cards)
    {
        Id = id; 
        Role = role; 
        Email = email; 
        Phone = phone; 
        Address = address; 
        Birthdate = birthDate;
        Cards = cards;
    }

    public void AddCard(CardModel card)
    {
        throw new NotImplementedException();
    }

    public void DeleteCard(int cardNr)
    {
        throw new NotImplementedException();
    }

>>>>>>> 3ffb26709e446be3ae26f44895a1099749156ecb
}
