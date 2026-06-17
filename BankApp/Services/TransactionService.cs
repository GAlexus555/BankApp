using BankApp.Models;
using BankApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace BankApp.Services;

public class TransactionService(HttpClient _client) : ITransactionService
{
    public async Task<TransactionModel?> CreateNewTransaction(string ibanFrom, string ibanTo, int amount, string description, TransactionStatus status)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/transactions/", new
            {
                amount_cents = amount,
                iban_from = ibanFrom,
                iban_to = ibanTo,
                description = description,
                status = status
            });
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<TransactionModel>();
        }
        catch (HttpRequestException) { return null; }
    }

    public async Task<List<TransactionModel>?> GetAllTransactions()
    {
        try
        {
            var response = await _client.GetAsync("/transactions/all");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<List<TransactionModel>>();
        }
        catch (HttpRequestException) { return null; }
    }

    public async Task<List<TransactionModel>?> GetMyTransactions()
    {
        try
        {
            var response = await _client.GetAsync("/transactions/");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<List<TransactionModel>>();
        }
        catch (HttpRequestException) { return null; }
    }

    public async Task<List<TransactionModel>?> GetTransactionsByAccountId(int accountId)
    {
        try
        {
            var response = await _client.GetAsync($"/transactions/account/{accountId}");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<List<TransactionModel>>();
        }
        catch (HttpRequestException) { return null; }
    }
}
