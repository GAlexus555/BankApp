using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Services.Interfaces;

public interface ICardService
{
    Task<List<CardModel>?> GetCardsAsync();
    Task<List<CardModel>?> GetAllCardsAsync();
    Task<CardModel> CreateCardAsync(int amountCents, CardStatus status, int ownerId, string iban, int cardNr, int cvc, DateTime expireDate);
}
