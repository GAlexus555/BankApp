using BankApp.Models;
using System.Collections.Generic;

namespace BankApp.Services.Interfaces;

public interface IStatsService
{
    Task<List<StatsModel>?> GetTransactionStatsAsync();
    Task<List<AuditLogModel>?> GetAuditLogsAsync();
}
