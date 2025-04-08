﻿using exSales.Domain.Impl.Services.Coins;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Services.Coins;
using exSales.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Impl.Factory
{
    public class CoinTxServiceFactory: ICoinTxServiceFactory
    {
        protected readonly IBtcTxService _btcTxService;
        protected readonly IStxTxService _stxTxService;
        protected readonly IUSDTTxService _usdtTxService;
        protected readonly IBRLTxService _brlTxService;

        public CoinTxServiceFactory(
            IBtcTxService btcTxService, 
            IStxTxService stxTxService,
            IUSDTTxService ustdTxService,
            IBRLTxService brlTxService
        )
        {
            _btcTxService = btcTxService;
            _stxTxService = stxTxService;
            _usdtTxService = ustdTxService;
            _brlTxService = brlTxService;
        }

        public ICoinTxService BuildCoinTxService(CoinEnum coin)
        {
            ICoinTxService coinTxService = null;
            switch (coin)
            {
                case CoinEnum.Bitcoin:
                    coinTxService = _btcTxService;
                    break;
                case CoinEnum.Stacks:
                    coinTxService = _stxTxService;
                    break;
                case CoinEnum.USDT:
                    coinTxService = _usdtTxService;
                    break;
                case CoinEnum.BRL:
                    coinTxService = _brlTxService;
                    break;
            }
            return coinTxService;
        }
    }
}
