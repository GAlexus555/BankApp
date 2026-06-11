using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BankApp.Models
{
    public class InterestModel
    {
        [JsonPropertyName("card_id")]
        public int card_id { get; set; }
        [JsonPropertyName("amount")]
        public int amount { get; set; }
        [JsonPropertyName("created_at")]
        public DateOnly createdAt { get; set; }
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("withdrawn")]
        public bool withdrawn { get; set; }
        [JsonPropertyName("interest_rate")]
        public double interest_rate { get; set; }

        public double CurrentAmount 
        { 
            get 
            {
                return GetCurrentAmount();
            }
        }

        private double GetCurrentAmount()
        {
            DateTime today = DateTime.Today;
            DateTime created = createdAt.ToDateTime(TimeOnly.MinValue);
            double years = (today - created).Days / 365.25;
            return amount * Math.Pow(1 + interest_rate, years);
        }
    }
}
