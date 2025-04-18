﻿using System;
using Core.Domain;
using DB.Infra.Context;
using exSales.Domain.Impl.Core;
using exSales.Domain.Interfaces.Core;

namespace DB.Infra
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ExSalesContext _goblinContext;
        private readonly ILogCore _log;

        public UnitOfWork(ILogCore log, ExSalesContext goblinContext)
        {
            this._goblinContext = goblinContext;
            _log = log;
        }

        public ITransaction BeginTransaction()
        {
            try
            {
                _log.Log("Iniciando bloco de transação.", Levels.Trace);
                return new TransactionDisposable(_log, _goblinContext.Database.BeginTransaction());
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
