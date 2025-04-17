using exSales.Domain.Interfaces.Models;
using exSales.DTO.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Services
{
    public interface INetworkService
    {
        INetworkModel Insert(NetworkInfo network);
    }
}
