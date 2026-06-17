using BankApp.Models;

namespace BankApp.Services.Interfaces;

public interface IBankService
{
    Task<BankModel?> GetBankAsync();
}
