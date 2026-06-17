using BankApp.Models;
using BankApp.Services.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;

namespace BankApp.Services;

public class BankService(HttpClient _client) : IBankService
{
    public async Task<BankModel?> GetBankAsync()
    {
        try
        {
            var response = await _client.GetAsync("/banks/");
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<BankModel>();
        }
        catch (HttpRequestException) { return null; }
    }
}
