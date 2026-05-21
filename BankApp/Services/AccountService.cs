using BankApp.Models;
using BankApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Services;

public class AccountService : IAccountService
{
    public async Task<AccountModel?> GetAccountAsync(Guid accountId)
    {
        var cards = await GetCardsAsync(accountId);

        var account = new AccountModel(
            accountId,
            AccountRole.Client,
            "chiara@gmail.com",
            "+43 676 67676767",
            "Fichtenweg 11",
            new DateTime(2009, 8, 10),
            cards);
            

        return await Task.FromResult<AccountModel?>(account);
    }

    public async Task<List<CardModel>?> GetCardsAsync(Guid accountId)
    {
        var cards = new List<CardModel>()
        {
            new CardModel("AT06 6767 6767 6767", "0450678067676767", new DateTime(2026, 12, 31), 454)
        };
        return cards ?? new List<CardModel>();
    }
}
