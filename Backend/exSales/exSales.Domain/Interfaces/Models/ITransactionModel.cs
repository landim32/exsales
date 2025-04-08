﻿using exSales.Domain.Impl.Models;
using exSales.Domain.Interfaces.Factory;
using exSales.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Models
{
    public interface ITransactionModel
    {
        long TxId { get; set; }
        long UserId { get; set; }
        string Hash { get; set; }
        ChainEnum Chain { get; set; }
        CoinEnum SenderCoin { get; set; }
        CoinEnum ReceiverCoin { get; set; }
        string RecipientAddress { get; set; }
        string SenderAddress { get; set; }
        string ReceiverAddress { get; set; }
        long? SenderTax { get; set; }
        long? ReceiverTax { get; set; }
        DateTime CreateAt { get; set; }
        DateTime UpdateAt { get; set; }
        TransactionStatusEnum Status { get; set; }
        string SenderTxid { get; set; }
        string ReceiverTxid { get; set; }
        long? SenderFee { get; set; }
        long? ReceiverFee { get; set; }
        long SenderAmount { get; set; }
        long ReceiverAmount { get; set; }

        string GetSenderCoinSymbol();
        string GetReceiverCoinSymbol();

        IUserModel GetUser(IUserDomainFactory factory);
        ITransactionModel Save();
        ITransactionModel Update();
        ITransactionModel GetBySenderAddr(string senderAddr, ITransactionDomainFactory factory);
        ITransactionModel GetByRecipientAddr(string recipientAddr, ITransactionDomainFactory factory);
        ITransactionModel GetById(long txId, ITransactionDomainFactory factory);
        ITransactionModel GetByHash(string hash, ITransactionDomainFactory factory);
        ITransactionModel GetBySenderTxId(string txid, ITransactionDomainFactory factory);
        ITransactionModel GetByReceiverTxId(string txid, ITransactionDomainFactory factory);
        IEnumerable<ITransactionModel> ListByAddress(string address, ITransactionDomainFactory factory);
        IEnumerable<ITransactionModel> ListByStatus(IList<int> status, ITransactionDomainFactory factory);
        IEnumerable<ITransactionModel> ListAll(ITransactionDomainFactory factory);
        IEnumerable<ITransactionModel> ListByUser(long userId, ITransactionDomainFactory factory);
        IEnumerable<ITransactionModel> ListToDetect(ITransactionDomainFactory factory);
    }
}
