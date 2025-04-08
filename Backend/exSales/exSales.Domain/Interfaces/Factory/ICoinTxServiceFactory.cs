using exSales.Domain.Interfaces.Services.Coins;
using exSales.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Factory
{
    public interface ICoinTxServiceFactory
    {
        ICoinTxService BuildCoinTxService(CoinEnum coin);
    }
}
