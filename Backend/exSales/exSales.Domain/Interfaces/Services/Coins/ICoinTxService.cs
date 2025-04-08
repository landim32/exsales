using exSales.Domain.Impl.Models;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using exSales.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Services.Coins
{
    public interface ICoinTxService
    {
        bool IsPaybackAutomatic();
        CoinEnum GetCoin();
        Task<string> GetPoolAddress();
        Task<string> GetNewAddress(int index);
        Task<long> GetPoolBalance();
        Task<TxResumeInfo> GetResumeTransaction(string txId);
        /*
        Task<bool> IsTransactionSuccessful(string txid);
        Task<long> GetSenderAmount(string txid, string senderAddr);
        Task<int> GetFee(string txid);
        */
        Task<string> Transfer(string address, long amount);
        //string GetSwapDescription(decimal proportion);
        string GetAddressUrl(string address);
        string GetTransactionUrl(string txId);
        string ConvertToString(decimal coin);
        Task<bool> VerifyTransaction(ITransactionModel tx);
        void AddLog(long txId, string msg, LogTypeEnum t, ITransactionLogDomainFactory txLogFactory);
        Task<IList<TxDetectedInfo>> DetectNewTransactions(string address);
    }
}
