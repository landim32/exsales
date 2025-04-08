using exSales.DTO.CoinMarketCap;
using exSales.DTO.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Services
{
    public interface ICoinMarketCapService
    {
        CoinSwapInfo GetCurrentPrice(CoinEnum senderCoin, CoinEnum receiverCoin, CurrencyEnum currency);
        decimal GetDollarPrice();
    }
}
