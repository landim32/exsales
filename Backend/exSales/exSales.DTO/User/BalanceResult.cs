using System;
using System.Text.Json.Serialization;
using exSales.DTO.Domain;

namespace exSales.DTO.User
{
    public class BalanceResult : StatusResult
    {
        [JsonPropertyName("balance")]
        public BalanceInfo Balance { get; set; }
    }
}
