using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Services.Interfaces
{
    public interface IInterestService
    {
        Task<List<InterestModel>?> GetMyInterestsAsync();
        Task<List<InterestModel>?> GetAllInterestsAsync();
        Task<InterestModel> CreateInterestAsync(int card_id, int amount_cents);
        Task<InterestModel> WithdrawInterestAsync(int interest_id);

    }
}
