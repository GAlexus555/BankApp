using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Models
{
    public class TransactionModel
    {
        public Guid ID { get; }
        public CardModel CardFrom { get; }
        public CardModel CardTo { get; }
        public double Amount { get; }
        public DateTime Date { get; }

        public TransactionModel(Guid id, CardModel cardFrom, CardModel cardTo, double amount, DateTime date)
        {
            ID = id;
            CardFrom = cardFrom;
            CardTo = cardTo;
            Amount = amount;
            Date = date;
        }
    }
}
