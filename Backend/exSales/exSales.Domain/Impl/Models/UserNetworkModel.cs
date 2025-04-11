using Core.Domain.Repository;
using Core.Domain;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using exSales.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Impl.Models
{
    public class UserNetworkModel : IUserNetworkModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserNetworkRepository<IUserNetworkModel, IUserNetworkDomainFactory> _repositoryNetwork;

        public UserNetworkModel(IUnitOfWork unitOfWork, IUserNetworkRepository<IUserNetworkModel, IUserNetworkDomainFactory> repositoryNetwork)
        {
            _unitOfWork = unitOfWork;
            _repositoryNetwork = repositoryNetwork;
        }

        public long UserId { get; set; }
        public long NetworkId { get; set; }
        public long ProfileId { get; set; }
        public UserRoleEnum Role { get; set; }
        public UserNetworkStatusEnum Status { get; set; }
        public long? ReferrerId { get; set; }

        public IUserNetworkModel Insert(IUserNetworkModel model, IUserNetworkDomainFactory factory)
        {
            return _repositoryNetwork.Insert(model, factory);
        }

        public IEnumerable<IUserNetworkModel> ListByUser(long userId, IUserNetworkDomainFactory factory)
        {
            return _repositoryNetwork.ListByUser(userId, factory);
        }

        public IUserNetworkModel Update(IUserNetworkModel model, IUserNetworkDomainFactory factory)
        {
            return _repositoryNetwork.Update(model, factory);
        }
    }
}
