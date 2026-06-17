using BankApp.Models;
using BankApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                amount = amount_cents
            });
            var body = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"POST /interests/ → {response.StatusCode}: {body}");
            if (!response.IsSuccessStatusCode) return null;
            return System.Text.Json.JsonSerializer.Deserialize<InterestModel>(body);
        }
        catch (Exception ex) { Debug.WriteLine($"CreateInterestAsync exception: {ex}"); return null; }
    }

    public async Task<List<InterestModel>?> GetAllInterestsAsync()
    {
        try
        {
            var response = await _client.GetAsync("/interests/all");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<List<InterestModel>>();
        }
        catch (HttpRequestException) { return null; }
    }

    public async Task<List<InterestModel>?> GetMyInterestsAsync()
    {
        try
        {
            var response = await _client.GetAsync("/interests/");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<List<InterestModel>>();
        }
        catch (HttpRequestException) { return null; }
    }

    public async Task<InterestModel> WithdrawInterestAsync(int interest_id)
    {
        try
        {
            var response = await _client.PostAsync($"/interests/{interest_id}/withdraw", null);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<InterestModel>();
        }
        catch (HttpRequestException) { return null; }
    }
}
