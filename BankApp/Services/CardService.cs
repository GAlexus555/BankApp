using BankApp.Models;
using BankApp.Services.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

namespace BankApp.Services;

public class CardService(HttpClient _client) : ICardService
{
    public async Task<List<CardModel>?> GetCardsAsync()
    {
        try
        {
            var response = await _client.GetAsync("/cards/");
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("GetCardsAsync failed — HTTP {StatusCode}", (int)response.StatusCode);
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<CardModel>>();
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error in GetCardsAsync");
            return null;
        }
    }

    public async Task<List<CardModel>?> GetCardsByAccountIdAsync(int accountId)
    {
        try
        {
            var response = await _client.GetAsync($"/cards/{accountId}");
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("GetCardsByAccountIdAsync failed for id={Id} — HTTP {StatusCode}",
                    accountId, (int)response.StatusCode);
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<CardModel>>();
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error getting cards for account id={Id}", accountId);
            return null;
        }
    }

    public async Task<List<CardModel>?> GetAllCardsAsync()
    {
        try
        {
            var response = await _client.GetAsync("/cards/all");
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("GetAllCardsAsync failed — HTTP {StatusCode}", (int)response.StatusCode);
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<CardModel>>();
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error in GetAllCardsAsync");
            return null;
        }
    }

    public async Task<CardModel?> CreateCardAsync(int amountCents, CardStatus status, int ownerId, string iban, string cardNr, int cvc, DateTime expireDate)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/cards/", new
            {
                cents       = amountCents,
                status      = (int)status,
                owner_id    = ownerId,
                iban        = iban,
                card_nr     = cardNr,
                cvc         = cvc,
                expire_date = expireDate.ToString("yyyy-MM-dd")
            });
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("CreateCardAsync failed for owner_id={OwnerId} — HTTP {StatusCode}",
                    ownerId, (int)response.StatusCode);
                return null;
            }
            var card = await response.Content.ReadFromJsonAsync<CardModel>();
            Log.Information("Card created: iban={IBAN}, owner_id={OwnerId}", iban, ownerId);
            return card;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error creating card for owner_id={OwnerId}", ownerId);
            return null;
        }
    }

    public async Task<bool> UpdateCardStatusAsync(int cardId, CardStatus status)
    {
        try
        {
            var response = await _client.PutAsJsonAsync($"/cards/{cardId}", new { status = (int)status });
            if (response.IsSuccessStatusCode)
            {
                Log.Information("Card status updated: id={CardId}, status={Status}", cardId, status);
                return true;
            }
            Log.Warning("UpdateCardStatusAsync failed for id={CardId} — HTTP {StatusCode}",
                cardId, (int)response.StatusCode);
            return false;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error updating card status id={CardId}", cardId);
            return false;
        }
    }

    public async Task<bool> DeleteCardAsync(int cardId)
    {
        try
        {
            var response = await _client.DeleteAsync($"/cards/{cardId}");
            if (response.IsSuccessStatusCode)
            {
                Log.Information("Card deleted: id={CardId}", cardId);
                return true;
            }
            Log.Warning("DeleteCardAsync failed for id={CardId} — HTTP {StatusCode}",
                cardId, (int)response.StatusCode);
            return false;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error deleting card id={CardId}", cardId);
            return false;
        }
    }
}
