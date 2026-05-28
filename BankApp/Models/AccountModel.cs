using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Models;

public enum AccountRole
{
    Client, 
    Manager
}

public class AccountModel
{
    public Guid Id { get; }
    public AccountRole Role { get; }
    public string Email { get; }
    public string Phone { get; }
    public string Address { get; }
    public DateTime Birthdate { get; }
    public List<CardModel>? Cards;

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

}
