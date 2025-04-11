using Core.Domain.Repository;
using Core.Domain;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using exSales.DTO.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Impl.Models
{
    public class NetworkModel : INetworkModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INetworkRepository<INetworkModel, INetworkDomainFactory> _repositoryNetwork;

        public NetworkModel(IUnitOfWork unitOfWork, INetworkRepository<INetworkModel, INetworkDomainFactory> repositoryNetwork)
        {
            _unitOfWork = unitOfWork;
            _repositoryNetwork = repositoryNetwork;
        }

        public long NetworkId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public double Commission { get; set; }
        public double WithdrawalMin { get; set; }
        public int WithdrawalPeriod { get; set; }
        public NetworkStatusEnum Status { get; set; }

        public INetworkModel Insert(INetworkModel model, INetworkDomainFactory factory)
        {
            return _repositoryNetwork.Insert(model, factory);
        }

        public INetworkModel Update(INetworkModel model, INetworkDomainFactory factory)
        {
            return _repositoryNetwork.Update(model, factory);
        }

        public IEnumerable<INetworkModel> ListAll(INetworkDomainFactory factory)
        {
            return _repositoryNetwork.ListAll(factory);
        }

        public INetworkModel GetById(long id, INetworkDomainFactory factory)
        {
            return _repositoryNetwork.GetById(id, factory);
        }
    }
}
