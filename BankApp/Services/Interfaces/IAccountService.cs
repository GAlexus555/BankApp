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
    Task<bool> DeleteAccountAsync(int id);
    Task<AccountModel?> CreateAccountAsync(string firstName, string lastName, string email, string password, string phone, string address, DateTime birthdate);
    Task<bool> UpdateAccountAsync(int id, string firstName, string lastName, string email, string phone, string address);
}