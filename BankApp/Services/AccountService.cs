using BankApp.Models;
using BankApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

namespace BankApp.Services;

public class AccountService(HttpClient _client, ICardService _cardService) : IAccountService
{
    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var form = new FormUrlEncodedContent([
                new("username", email),
                new("password", password)
            ]);

            var response = await _client.PostAsync("/accounts/login", form);
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            var token = result?.AccessToken;

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return token != null;
        }
        catch (HttpRequestException) { return false; }
    }

    public void Logout()
    {
        _client.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<AccountModel?> GetMeAsync()
    {
        try
        {
            var response = await _client.GetAsync("/accounts/me");
            if (!response.IsSuccessStatusCode) return null;

            var account = await response.Content.ReadFromJsonAsync<AccountModel>();
            if (account != null)
                account.Cards = await _cardService.GetCardsAsync();
            return account;
        }
        catch (HttpRequestException) { return null; }
    }

    public async Task<List<AccountModel>?> GetAllAccountsAsync()
    {
        try
        {
            var response = await _client.GetAsync("/accounts/");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<List<AccountModel>>();
        }
        catch (HttpRequestException) { return null; }
    }

    public async Task<bool> DeleteAccountAsync(int id)
    {
        try
        {
            var response = await _client.DeleteAsync($"/accounts/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException) { return false; }
    }

    public async Task<AccountModel?> CreateAccountAsync(string firstName, string lastName, string email, string password, string phone, string address, DateTime birthdate, int role = 0)
    {
        try
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
                role = role
            });
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<AccountModel>();
        }
        catch (HttpRequestException) { return null; }
    }

    public async Task<bool> UpdateMeAsync(string firstName, string lastName, string email, string phone, string address, DateTime birthdate, string password)
    {
        try
        {
            var response = await _client.PutAsJsonAsync("/accounts/me", new
            {
                firstname = firstName,
                lastname = lastName,
                email = email,
                phonenumber = phone,
                address = address,
                birthdate = birthdate.ToString("yyyy-MM-dd"),
                password = password,
                role = 0
            });
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException) { return false; }
    }

    public async Task<bool> UpdateAccountAsync(int id, string firstName, string lastName, string email, string phone, string address, DateTime birthdate, string password, int role = 0)
    {
        try
        {
            var response = await _client.PutAsJsonAsync($"/accounts/{id}", new
            {
                firstname = firstName,
                lastname = lastName,
                email = email,
                phonenumber = phone,
                address = address,
                birthdate = birthdate.ToString("yyyy-MM-dd"),
                password = password,
                role = role
            });
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException) { return false; }
    }
}
