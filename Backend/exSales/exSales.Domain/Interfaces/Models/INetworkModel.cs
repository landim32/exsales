using exSales.Domain.Interfaces.Factory;
using exSales.DTO.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Models
{
    public interface INetworkModel
    {
        long NetworkId { get; set; }

        string Name { get; set; }

        string Email { get; set; }

        double Commission { get; set; }

        double WithdrawalMin { get; set; }

        int WithdrawalPeriod { get; set; }

        NetworkStatusEnum Status { get; set; }

        IEnumerable<INetworkModel> ListAll(INetworkDomainFactory factory);
        INetworkModel GetById(long id, INetworkDomainFactory factory);
        INetworkModel Insert(INetworkModel model, INetworkDomainFactory factory);
        INetworkModel Update(INetworkModel model, INetworkDomainFactory factory);
    }
}
