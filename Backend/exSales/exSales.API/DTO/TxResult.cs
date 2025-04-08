﻿using System.Text.Json.Serialization;

namespace exSales.API.DTO
{
    public class TxResult
    {
        [JsonPropertyName("txid")]
        public long TxId { get; set; }
        [JsonPropertyName("hash")]
        public string Hash { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("sendercoin")]
        public string SenderCoin { get; set; }
        [JsonPropertyName("receivercoin")]
        public string ReceiverCoin { get; set; }
        [JsonPropertyName("recipientaddress")]
        public string RecipientAddress { get; set; }
        [JsonPropertyName("senderaddress")]
        public string SenderAddress { get; set; }
        [JsonPropertyName("senderaddressurl")]
        public string SenderAddressUrl { get; set; }
        [JsonPropertyName("receiveraddress")]
        public string ReceiverAddress { get; set; }
        [JsonPropertyName("receiveraddressurl")]
        public string ReceiverAddressUrl { get; set; }
        [JsonPropertyName("createat")]
        public string CreateAt { get; set; }
        [JsonPropertyName("updateat")]
        public string UpdateAt { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("sendertxid")]
        public string SenderTxid { get; set; }
        [JsonPropertyName("sendertxidurl")]
        public string SenderTxidUrl { get; set; }
        [JsonPropertyName("receivertxid")]
        public string ReceiverTxid { get; set; }
        [JsonPropertyName("receivertxidurl")]
        public string ReceiverTxidUrl { get; set; }
        [JsonPropertyName("senderfee")]
        public string SenderFee { get; set; }
        [JsonPropertyName("receiverfee")]
        public string ReceiverFee { get; set; }

        [JsonPropertyName("sendertax")]
        public string SenderTax { get; set; }
        [JsonPropertyName("receivertax")]
        public string ReceiverTax { get; set; }

        [JsonPropertyName("senderamount")]
        public string SenderAmount { get; set; }
        [JsonPropertyName("receiveramount")]
        public string ReceiverAmount { get; set; }
        [JsonPropertyName("senderamountvalue")]
        public long SenderAmountValue { get; set; }
        [JsonPropertyName("receiverpayback")]
        public long ReceiverPayback { get; set; }
    }
}
