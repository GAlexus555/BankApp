using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Models
{
    public class BankModel
    {
        public string Name { get; }
        public string Address { get; }
        public List<AccountModel>? Accounts;
        public List<TransactionModel>? Transactions;
        public double InterestPercentage { get; }
        public List<InterestAccountModel>? InterestAccounts;

        public BankModel(string name, string address, List<AccountModel>? accounts, List<TransactionModel>? transactions, double interestPercentage, List<InterestAccountModel>? interestAccounts)
        {
            Name = name;
            Address = address;
            Accounts = accounts;
            Transactions = transactions;
            InterestPercentage = interestPercentage;
            InterestAccounts = interestAccounts;
        }

        public void CreateAccount(AccountModel acc)
        {
            throw new NotImplementedException();
        }

        public void DeleteAccount(int id)
        {
            Accounts?.RemoveAll(a => a.Id == id);
        }

        public void UpdateAccount(AccountModel acc)
        {
            var existing = Accounts?.FirstOrDefault(a => a.Id == acc.Id);
            if (existing == null) return;

            existing.FirstName = acc.FirstName;
            existing.LastName = acc.LastName;
            existing.Email = acc.Email;
            existing.Phone = acc.Phone;
            existing.Address = acc.Address;
        }
    }
}
