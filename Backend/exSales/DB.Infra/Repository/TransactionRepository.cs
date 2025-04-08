﻿using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using exSales.DTO.Transaction;
using Core.Domain.Repository;
using DB.Infra.Context;
using NoobsMuc.Coinmarketcap.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exSales.Domain.Impl.Core;
using System.Net;

namespace DB.Infra.Repository
{
    public class TransactionRepository : ITransactionRepository<ITransactionModel, ITransactionDomainFactory>
    {
        private NoChainSwapContext _ccsContext;

        public TransactionRepository(NoChainSwapContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private ITransactionModel DbToModel(ITransactionDomainFactory factory, Transaction u)
        {
            var md = factory.BuildTransactionModel();
            md.TxId = u.TxId;
            md.UserId = u.UserId;
            md.Hash = u.Hash;
            md.SenderCoin = Utils.StrToCoin(u.SenderCoin);
            md.ReceiverCoin = Utils.StrToCoin(u.ReceiverCoin);
            md.RecipientAddress = u.RecipientAddress;
            md.SenderAddress = u.SenderAddress;
            md.ReceiverAddress = u.ReceiverAddress;
            md.CreateAt = u.CreateAt;
            md.UpdateAt = u.UpdateAt;
            md.Status = (TransactionStatusEnum)u.Status;
            md.SenderTxid = u.SenderTxid;
            md.ReceiverTxid = u.ReceiverTxid;
            md.SenderFee = u.SenderFee;
            md.ReceiverFee = u.ReceiverFee;
            md.SenderTax = u.SenderTax;
            md.ReceiverTax = u.ReceiverTax;
            md.SenderAmount = u.SenderAmount;
            md.ReceiverAmount = u.ReceiverAmount;
            return md;
        }

        private void ModelToDb(ITransactionModel u, Transaction md)
        {
            md.TxId = u.TxId;
            md.UserId = u.UserId;
            md.Hash = u.Hash;
            md.SenderCoin = Utils.CoinToStr(u.SenderCoin);
            md.ReceiverCoin = Utils.CoinToStr(u.ReceiverCoin);
            md.RecipientAddress = u.RecipientAddress;
            md.SenderAddress = u.SenderAddress;
            md.ReceiverAddress = u.ReceiverAddress;
            md.CreateAt = u.CreateAt;
            md.UpdateAt = u.UpdateAt;
            md.Status = (int)u.Status;
            md.SenderTxid = u.SenderTxid;
            md.ReceiverTxid = u.ReceiverTxid;
            md.SenderFee = u.SenderFee;
            md.ReceiverFee = u.ReceiverFee;
            md.SenderTax = u.SenderTax;
            md.ReceiverTax = u.ReceiverTax;
            md.SenderAmount = u.SenderAmount;
            md.ReceiverAmount = u.ReceiverAmount;
        }

        public ITransactionModel GetBySenderAddr(string senderAddr, ITransactionDomainFactory factory)
        {
            var row = _ccsContext.Transactions.Where(x => x.SenderAddress.ToLower() == senderAddr.ToLower()).FirstOrDefault();
            if (row != null)
            {
                return DbToModel(factory, row);
            }
            return null;
        }

        public ITransactionModel GetByRecipientAddr(string recipientAddr, ITransactionDomainFactory factory)
        {
            var row = _ccsContext.Transactions.Where(x => x.RecipientAddress.ToLower() == recipientAddr.ToLower()).FirstOrDefault();
            if (row != null)
            {
                return DbToModel(factory, row);
            }
            return null;
        }

        public ITransactionModel GetById(long txId, ITransactionDomainFactory factory)
        {
            try
            {
                var row = _ccsContext.Transactions.Where(x => x.TxId == txId).FirstOrDefault();
                if (row != null)
                {
                    return DbToModel(factory, row);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ITransactionModel GetByHash(string hash, ITransactionDomainFactory factory)
        {
            try
            {
                var row = _ccsContext.Transactions.Where(x => x.Hash.ToLower() == hash.ToLower()).FirstOrDefault();
                if (row != null)
                {
                    return DbToModel(factory, row);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<ITransactionModel> ListByUser(long userId, ITransactionDomainFactory factory)
        {
            var rows = _ccsContext.Transactions.Where(x => x.UserId == userId).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IEnumerable<ITransactionModel> ListByAddress(string address, ITransactionDomainFactory factory)
        {
            var rows = _ccsContext.Transactions.Where(x => x.SenderAddress.ToLower() == address.ToLower() || x.ReceiverAddress.ToLower() == address.ToLower()).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IEnumerable<ITransactionModel> ListByStatus(IList<int> status, ITransactionDomainFactory factory)
        {
            var rows = _ccsContext.Transactions.Where(x => status.Contains(x.Status)).OrderBy(x => x.CreateAt).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IEnumerable<ITransactionModel> ListAll(ITransactionDomainFactory factory)
        {
            var rows = _ccsContext.Transactions.OrderByDescending(x => x.UpdateAt).Take(100).ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public ITransactionModel SaveTx(ITransactionModel model)
        {
            try
            {
                var u = new Transaction();
                ModelToDb(model, u);

                _ccsContext.Add(u);
                _ccsContext.SaveChanges();
                model.TxId = u.TxId;
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ITransactionModel UpdateTx(ITransactionModel model)
        {
            try
            {
                var row = _ccsContext.Transactions.Where(x => x.TxId == model.TxId).FirstOrDefault();
                ModelToDb(model, row);
                _ccsContext.Transactions.Update(row);
                _ccsContext.SaveChanges();
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ITransactionModel GetBySenderTxId(string txid, ITransactionDomainFactory factory)
        {
            try
            {
                var row = _ccsContext.Transactions.Where(x => x.SenderTxid.ToLower() == txid.ToLower()).FirstOrDefault();
                if (row != null)
                {
                    return DbToModel(factory, row);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ITransactionModel GetByReceiverTxId(string txid, ITransactionDomainFactory factory)
        {
            try
            {
                var row = _ccsContext.Transactions.Where(x => x.ReceiverTxid.ToLower() == txid.ToLower()).FirstOrDefault();
                if (row != null)
                {
                    return DbToModel(factory, row);
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<ITransactionModel> ListToDetect(ITransactionDomainFactory factory)
        {
            return _ccsContext.Transactions.Where(x =>
                   x.Status == (int)TransactionStatusEnum.WaitingSenderPayment
                   && !string.IsNullOrEmpty(x.RecipientAddress)
                   && string.IsNullOrEmpty(x.SenderTxid)
                   && x.SenderCoin != Utils.CoinToStr(CoinEnum.BRL)
            )
            .ToList()
            .Select(x => DbToModel(factory, x))
            .ToList();
        }
    }
}
