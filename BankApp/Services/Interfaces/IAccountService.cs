using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Services.Interfaces;

public interface IAccountService
{
    Task<AccountModel?> GetAccountAsync(Guid accountId);
    Task<List<CardModel>?> GetCardsAsync(Guid accountId);
}
