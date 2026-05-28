using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Models
{
    public class InterestAccountModel
    {
        public Guid ID { get; }
        public CardModel Card { get; }
        public double Amount { get; }

        public InterestAccountModel(Guid id, CardModel card, double amount)
        {
            ID = id;
            Card = card;
            Amount = amount;
        }
    }
}
