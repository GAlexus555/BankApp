using BankApp.Models;
using BankApp.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

namespace BankApp.Services;

public class StatsService(HttpClient _client) : IStatsService
{
    public async Task<List<StatsModel>?> GetTransactionStatsAsync()
    {
        try
        {
            var r = await _client.GetAsync("/stats/transactions-per-account");
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<List<StatsModel>>();
        }
        catch (HttpRequestException) { return null; }
    }

    public async Task<List<AuditLogModel>?> GetAuditLogsAsync()
    {
        try
        {
            var r = await _client.GetAsync("/audit-logs/");
            if (!r.IsSuccessStatusCode) return null;
            return await r.Content.ReadFromJsonAsync<List<AuditLogModel>>();
        }
        catch (HttpRequestException) { return null; }
    }
}
