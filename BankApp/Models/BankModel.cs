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

        public void DeleteAccount() //
        {
            throw new NotImplementedException();
        }

        public void UpdateAccount(AccountModel acc) //
        {
            throw new NotImplementedException();
        }
    }
}
