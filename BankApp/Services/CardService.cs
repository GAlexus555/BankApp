using BankApp.Models;
using BankApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Services;

public class CardService : ICardService
{
    public Task<CardModel> CreateCardAsync(int amountCents, CardStatus status, int ownerId, string iban, int cardNr, int cvc, DateTime expireDate)
    {
        throw new NotImplementedException();
    }

    public Task<List<CardModel>?> GetAllCardsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<CardModel>?> GetCardsAsync()
    {
        throw new NotImplementedException();
    }
}
