using BankApp.Models;
using System;
using System.Collections.Generic;

namespace BankApp.Services.Interfaces;

public interface ICardService
{
    Task<List<CardModel>?> GetCardsAsync();
    Task<List<CardModel>?> GetAllCardsAsync();
    Task<List<CardModel>?> GetCardsByAccountIdAsync(int accountId);
    Task<CardModel?> CreateCardAsync(int amountCents, CardStatus status, int ownerId, string iban, string cardNr, int cvc, DateTime expireDate);
    Task<bool> UpdateCardStatusAsync(int cardId, CardStatus status);
    Task<bool> DeleteCardAsync(int cardId);
}
