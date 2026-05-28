using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Services.Interfaces;

public interface IAccountService
{
    Task<bool> LoginAsync(string email, string password);
    Task<AccountModel?> GetMeAsync();
    Task<List<AccountModel>?> GetAllAccountsAsync();
    Task<List<CardModel>?> GetCardsAsync(int accountId);
}
