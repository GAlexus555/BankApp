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

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        return _token != null;
    }

    public async Task<List<CardModel>?> GetCardsAsync(int accountId)
    {
        var response = await _client.GetAsync("/cards/");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<List<CardModel>>();
    }

    public async Task<AccountModel?> GetMeAsync()
    {
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

    public async Task<bool> DeleteAccountAsync(int id)
    {
        var response = await _client.DeleteAsync($"/accounts/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<AccountModel?> CreateAccountAsync(string firstName, string lastName, string email, string password, string phone, string address, DateTime birthdate)
    {
        var response = await _client.PostAsJsonAsync("/accounts/register", new
        {
            firstname = firstName,
            lastname = lastName,
            email = email,
            password = password,
            phonenumber = phone,
            address = address,
            birthdate = birthdate.ToString("yyyy-MM-dd"),
            role = 0
        });
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AccountModel>();
    }

    public async Task<bool> UpdateAccountAsync(int id, string firstName, string lastName, string email, string phone, string address)
    {
        var response = await _client.PutAsJsonAsync($"/accounts/{id}", new
        {
            firstname = firstName,
            lastname = lastName,
            email = email,
            phonenumber = phone,
            address = address
        });
        return response.IsSuccessStatusCode;
    }
}