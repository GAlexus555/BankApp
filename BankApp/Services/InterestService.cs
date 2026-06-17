using BankApp.Models;
using BankApp.Services.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

namespace BankApp.Services;

public class InterestService(HttpClient _client) : IInterestService
{
    public async Task<InterestModel> CreateInterestAsync(int card_id, int amount_cents)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/interests/", new
            {
                card_id = card_id,
                amount  = amount_cents
            });
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("CreateInterestAsync failed for card_id={CardId} — HTTP {StatusCode}",
                    card_id, (int)response.StatusCode);
                return null;
            }
            var interest = await response.Content.ReadFromJsonAsync<InterestModel>();
            Log.Information("Interest created: card_id={CardId}, amount={Amount} ct", card_id, amount_cents);
            return interest;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error creating interest for card_id={CardId}", card_id);
            return null;
        }
    }

    public async Task<List<InterestModel>?> GetAllInterestsAsync()
    {
        try
        {
            var response = await _client.GetAsync("/interests/all");
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("GetAllInterestsAsync failed — HTTP {StatusCode}", (int)response.StatusCode);
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<InterestModel>>();
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error in GetAllInterestsAsync");
            return null;
        }
    }

    public async Task<List<InterestModel>?> GetMyInterestsAsync()
    {
        try
        {
            var response = await _client.GetAsync("/interests/");
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("GetMyInterestsAsync failed — HTTP {StatusCode}", (int)response.StatusCode);
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<InterestModel>>();
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error in GetMyInterestsAsync");
            return null;
        }
    }

    public async Task<InterestModel> WithdrawInterestAsync(int interest_id)
    {
        try
        {
            var response = await _client.PostAsync($"/interests/{interest_id}/withdraw", null);
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("WithdrawInterestAsync failed for id={Id} — HTTP {StatusCode}",
                    interest_id, (int)response.StatusCode);
                return null;
            }
            var interest = await response.Content.ReadFromJsonAsync<InterestModel>();
            Log.Information("Interest withdrawn: id={Id}", interest_id);
            return interest;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error withdrawing interest id={Id}", interest_id);
            return null;
        }
    }
}
