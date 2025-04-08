using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.DTO.Mempool
{
    public class UtxoInfo
    {
        [JsonProperty("txid")]
        public string Txid { get; set; }

        [JsonProperty("vout")]
        public int Vout { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }
}
