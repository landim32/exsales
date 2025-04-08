﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace exSales.DTO.Transaction
{
    public class TransactionParamInfo
    {
        [JsonPropertyName("userid")]
        public long UserId { get; set; }
        [JsonPropertyName("sendercoin")]
        public string SenderCoin { get; set; }
        [JsonPropertyName("receivercoin")]
        public string ReceiverCoin { get; set; }
        [JsonPropertyName("senderaddress")]
        public string SenderAddress { get; set; }
        [JsonPropertyName("receiveraddress")]
        public string ReceiverAddress { get; set; }
        [JsonPropertyName("sendertxid")]
        public string SenderTxid { get; set; }
        [JsonPropertyName("senderamount")]
        public long SenderAmount { get; set; }
        [JsonPropertyName("receiveramount")]
        public long ReceiverAmount { get; set; }
    }
}
