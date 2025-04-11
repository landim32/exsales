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
    public class UserNetworkDomainFactory : IUserNetworkDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserNetworkRepository<IUserNetworkModel, IUserNetworkDomainFactory> _repositoryNetwork;

        public UserNetworkDomainFactory(IUnitOfWork unitOfWork, IUserNetworkRepository<IUserNetworkModel, IUserNetworkDomainFactory> repositoryNetwork)
        {
            _unitOfWork = unitOfWork;
            _repositoryNetwork = repositoryNetwork;
        }

        public IUserNetworkModel BuildUserNetworkModel()
        {
            return new UserNetworkModel(_unitOfWork, _repositoryNetwork);
        }
    }
}
