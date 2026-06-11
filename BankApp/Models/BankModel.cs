using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace BankApp.Models
{
    public class BankModel
    {
        [JsonPropertyName("bankname")]
        public string BankName { get; set; }
        [JsonPropertyName("interest_rate")]
        public int Interestrate { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
