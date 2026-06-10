using BankApp.Models;
using BankApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;

namespace BankApp.Services;

public class CardService(HttpClient _client) : ICardService
{
    public async Task<List<CardModel>?> GetCardsAsync()
    {
        var response = await _client.GetAsync("/cards/");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<List<CardModel>>();
    }

    public async Task<List<CardModel>?> GetCardsByAccountIdAsync(int accountId)
    {
        var response = await _client.GetAsync($"/cards/{accountId}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<List<CardModel>>();
    }

    public async Task<List<CardModel>?> GetAllCardsAsync()
    {
        var response = await _client.GetAsync("/cards/all/");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<List<CardModel>>();
    }

    public async Task<CardModel?> CreateCardAsync(int amountCents, CardStatus status, int ownerId, string iban, string cardNr, int cvc, DateTime expireDate)
    {
        var response = await _client.PostAsJsonAsync("/cards/", new
        {
            cents = amountCents,
            status = (int)status,
            owner_id = ownerId,
            iban = iban,
            card_nr = cardNr,
            cvc = cvc,
            expire_date = expireDate.ToString("yyyy-MM-dd")
        });
        var body = await response.Content.ReadAsStringAsync();
        Debug.WriteLine($"POST /cards/ → {response.StatusCode}: {body}");
        if (!response.IsSuccessStatusCode) return null;
        return System.Text.Json.JsonSerializer.Deserialize<CardModel>(body);
    }
}
