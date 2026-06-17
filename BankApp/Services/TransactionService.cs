using BankApp.Models;
using BankApp.Services.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

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
                iban_from    = ibanFrom,
                iban_to      = ibanTo,
                description  = description,
                status       = status
            });
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("Transaction failed: {From} → {To}, {Amount} ct — HTTP {StatusCode}",
                    ibanFrom, ibanTo, amount, (int)response.StatusCode);
                return null;
            }
            var tx = await response.Content.ReadFromJsonAsync<TransactionModel>();
            Log.Information("Transaction created: {From} → {To}, {Amount} ct", ibanFrom, ibanTo, amount);
            return tx;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error creating transaction {From} → {To}", ibanFrom, ibanTo);
            return null;
        }
    }

    public async Task<List<TransactionModel>?> GetAllTransactions()
    {
        try
        {
            var response = await _client.GetAsync("/transactions/all");
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("GetAllTransactions failed — HTTP {StatusCode}", (int)response.StatusCode);
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<TransactionModel>>();
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error in GetAllTransactions");
            return null;
        }
    }

    public async Task<List<TransactionModel>?> GetMyTransactions()
    {
        try
        {
            var response = await _client.GetAsync("/transactions/");
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("GetMyTransactions failed — HTTP {StatusCode}", (int)response.StatusCode);
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<TransactionModel>>();
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error in GetMyTransactions");
            return null;
        }
    }

    public async Task<List<TransactionModel>?> GetTransactionsByAccountId(int accountId)
    {
        try
        {
            var response = await _client.GetAsync($"/transactions/account/{accountId}");
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("GetTransactionsByAccountId failed for id={Id} — HTTP {StatusCode}",
                    accountId, (int)response.StatusCode);
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<TransactionModel>>();
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error getting transactions for account id={Id}", accountId);
            return null;
        }
    }
}
