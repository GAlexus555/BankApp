using BankApp.Models;
using BankApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace BankApp.Services;

public class AccountService(HttpClient _client) : IAccountService
{
    private string? _token;

    public async Task<bool> LoginAsync(string email, string password)
    {
        var form = new FormUrlEncodedContent([
            new("username", email),
            new("password", password)
        ]);

        var response = await _client.PostAsync("/accounts/login", form);
        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        _token = result?.AccessToken;

        // Setzt token für jeder zukünftige Http Request. Angenehmend :)
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        return _token != null;
    }

    public async Task<List<CardModel>?> GetCardsAsync(int accountId)
    {
        var cards = new List<CardModel>()
        {
            new CardModel("AT06 6767 6767 6767", "0450678067676767", new DateTime(2026, 12, 31), 454)
        };
        return cards ?? new List<CardModel>();
    }

    public async Task<AccountModel?> GetMeAsync()
    {
        // Token is already set in DefaultRequestHeaders after login
        var response = await _client.GetAsync("/accounts/me");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<AccountModel>();
    }

    public async Task<List<AccountModel>?> GetAllAccountsAsync()
    {
        var response = await _client.GetAsync("/accounts/");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<List<AccountModel>>();
    }
}
