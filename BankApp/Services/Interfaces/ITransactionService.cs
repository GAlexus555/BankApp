using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Services.Interfaces;

public interface ITransactionService
{
    Task<TransactionModel?> CreateNewTransaction(string ibanFrom, string ibanTo, int amount, string description, TransactionStatus status);
    Task<List<TransactionModel>?> GetAllTransactions();
    Task<List<TransactionModel>?> GetMyTransactions();
    Task<List<TransactionModel>?> GetTransactionsByAccountId(int accountId);
}
