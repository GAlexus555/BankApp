using BankApp.Models;
using BankApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace BankApp.Services;

public class TransactionService(HttpClient _client) : ITransactionService
{
    public async Task<TransactionModel?> CreateNewTransaction(string ibanFrom, string ibanTo, int amount, string description, TransactionStatus status)
    {
        var response = await _client.PostAsJsonAsync("/transactions/", new
        {
            amount_cents = amount,
            iban_from = ibanFrom,
            iban_to = ibanTo,
            description = description,
            status = status
        });
        return await response.Content.ReadFromJsonAsync<TransactionModel>();
    }

    public async Task<List<TransactionModel>?> GetAllTransactions()
    {
        var response = await _client.GetAsync("/transactions/all");

        Debug.WriteLine(response.StatusCode);
        Debug.WriteLine(_client.DefaultRequestHeaders.Authorization);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        Debug.WriteLine(json);

        return System.Text.Json.JsonSerializer.Deserialize<List<TransactionModel>>(json);
    }

    public async Task<List<TransactionModel>?> GetMyTransactions()
    {
        var response = await _client.GetAsync("/transactions/");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<List<TransactionModel>>();
    }

    public async Task<List<TransactionModel>?> GetTransactionsByAccountId(int accountId)
    {
        var response = await _client.GetAsync($"/transactions/?account_id={accountId}");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<List<TransactionModel>>();
    }
}
