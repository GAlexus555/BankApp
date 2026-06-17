using BankApp.Models;
using BankApp.Services.Interfaces;
using Serilog;
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
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("Login failed for {Email} — HTTP {StatusCode}", email, (int)response.StatusCode);
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            var token = result?.AccessToken;

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            Log.Information("Login successful for {Email}", email);
            return token != null;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error during login for {Email}", email);
            return false;
        }
    }

    public void Logout()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        Log.Information("User logged out");
    }

    public async Task<AccountModel?> GetMeAsync()
    {
        try
        {
            var response = await _client.GetAsync("/accounts/me");
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("GetMeAsync failed — HTTP {StatusCode}", (int)response.StatusCode);
                return null;
            }

            var account = await response.Content.ReadFromJsonAsync<AccountModel>();
            if (account != null)
                account.Cards = await _cardService.GetCardsAsync();
            return account;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error in GetMeAsync");
            return null;
        }
    }

    public async Task<List<AccountModel>?> GetAllAccountsAsync()
    {
        try
        {
            var response = await _client.GetAsync("/accounts/");
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("GetAllAccountsAsync failed — HTTP {StatusCode}", (int)response.StatusCode);
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<AccountModel>>();
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error in GetAllAccountsAsync");
            return null;
        }
    }

    public async Task<bool> DeleteAccountAsync(int id)
    {
        try
        {
            var response = await _client.DeleteAsync($"/accounts/{id}");
            if (response.IsSuccessStatusCode)
            {
                Log.Information("Account deleted: id={Id}", id);
                return true;
            }
            Log.Warning("DeleteAccountAsync failed for id={Id} — HTTP {StatusCode}", id, (int)response.StatusCode);
            return false;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error deleting account id={Id}", id);
            return false;
        }
    }

    public async Task<AccountModel?> CreateAccountAsync(string firstName, string lastName, string email, string password, string phone, string address, DateTime birthdate, int role = 0)
    {
        try
        {
            var response = await _client.PostAsJsonAsync("/accounts/register", new
            {
                firstname  = firstName,
                lastname   = lastName,
                email      = email,
                password   = password,
                phonenumber = phone,
                address    = address,
                birthdate  = birthdate.ToString("yyyy-MM-dd"),
                role       = role
            });
            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("CreateAccountAsync failed for {Email} — HTTP {StatusCode}", email, (int)response.StatusCode);
                return null;
            }
            var account = await response.Content.ReadFromJsonAsync<AccountModel>();
            Log.Information("Account created: {Email} (role={Role})", email, role);
            return account;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error creating account for {Email}", email);
            return null;
        }
    }

    public async Task<bool> UpdateMeAsync(string firstName, string lastName, string email, string phone, string address, DateTime birthdate, string password)
    {
        try
        {
            var response = await _client.PutAsJsonAsync("/accounts/me", new
            {
                firstname   = firstName,
                lastname    = lastName,
                email       = email,
                phonenumber = phone,
                address     = address,
                birthdate   = birthdate.ToString("yyyy-MM-dd"),
                password    = password,
                role        = 0
            });
            if (response.IsSuccessStatusCode)
            {
                Log.Information("Own profile updated for {Email}", email);
                return true;
            }
            Log.Warning("UpdateMeAsync failed — HTTP {StatusCode}", (int)response.StatusCode);
            return false;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error in UpdateMeAsync");
            return false;
        }
    }

    public async Task<bool> UpdateAccountAsync(int id, string firstName, string lastName, string email, string phone, string address, DateTime birthdate, string password, int role = 0)
    {
        try
        {
            var response = await _client.PutAsJsonAsync($"/accounts/{id}", new
            {
                firstname   = firstName,
                lastname    = lastName,
                email       = email,
                phonenumber = phone,
                address     = address,
                birthdate   = birthdate.ToString("yyyy-MM-dd"),
                password    = password,
                role        = role
            });
            if (response.IsSuccessStatusCode)
            {
                Log.Information("Account updated: id={Id}, email={Email}", id, email);
                return true;
            }
            Log.Warning("UpdateAccountAsync failed for id={Id} — HTTP {StatusCode}", id, (int)response.StatusCode);
            return false;
        }
        catch (HttpRequestException ex)
        {
            Log.Error(ex, "Network error updating account id={Id}", id);
            return false;
        }
    }
}
