using Core.Domain.Repository;
using Core.Domain;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exSales.Domain.Impl.Models;

namespace exSales.Domain.Impl.Factory
{
    public class NetworkDomainFactory : INetworkDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INetworkRepository<INetworkModel, INetworkDomainFactory> _repositoryNetwork;

        public NetworkDomainFactory(IUnitOfWork unitOfWork, INetworkRepository<INetworkModel, INetworkDomainFactory> repositoryNetwork)
        {
            _unitOfWork = unitOfWork;
            _repositoryNetwork = repositoryNetwork;
        }

        public INetworkModel BuildNetworkModel()
        {
            return new NetworkModel(_unitOfWork, _repositoryNetwork);
        }
    }
}
