using BankApp.Models;
using BankApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace BankApp.Services
{
    public class InterestService(HttpClient _client) : IInterestService
    {
        public async Task<InterestModel> CreateInterestAsync(int _card_id, int amount_cents)
        {
            var response = await _client.PostAsJsonAsync("/interests/", new
            {
                card_id = _card_id,
                amount = amount_cents
            });
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return null;
            return System.Text.Json.JsonSerializer.Deserialize<InterestModel>(body);
        }

        public async Task<List<InterestModel>?> GetAllInterestsAsync()
        {
            var response = await _client.GetAsync("/interests/all");
            if (response.IsSuccessStatusCode == false)
            {
                return null;
            }
            else return await response.Content.ReadFromJsonAsync<List<InterestModel>>();
        }

        public async Task<List<InterestModel>?> GetMyInterestsAsync()
        {
            var respone = await _client.GetAsync("/interests/");
            if (respone.IsSuccessStatusCode == false)
            {
                return null;
            }
            else return await respone.Content.ReadFromJsonAsync<List<InterestModel>>();
        }

        public async Task<InterestModel> WithdrawInterestAsync(int interest_id)
        {
            var response = await _client.PostAsync($"/interests/{interest_id}/withdraw", null);

            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode) return null;
            return System.Text.Json.JsonSerializer.Deserialize<InterestModel>(body);
        }
    }
}
