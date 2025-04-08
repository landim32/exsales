using exSales.DTO.Mempool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Services
{
    public interface IMempoolService
    {
        Task<long> GetBalance(string address);
        Task<TxRecommendedFeeInfo> GetRecommendedFee();
        Task<IList<UtxoInfo>> ListUTXO(string address);
        Task<MemPoolTxInfo> GetTransaction(string txid);
        Task<string> BroadcastTransaction(string hexTx);
    }
}
