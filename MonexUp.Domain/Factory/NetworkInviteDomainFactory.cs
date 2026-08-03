using Core.Domain;
using Core.Domain.Repository;
using MonexUp.Domain.Impl.Models;
using MonexUp.Domain.Interfaces.Factory;
using MonexUp.Domain.Interfaces.Models;

namespace MonexUp.Domain.Impl.Factory
{
    public class NetworkInviteDomainFactory : INetworkInviteDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INetworkInviteRepository<INetworkInviteModel, INetworkInviteDomainFactory> _repository;

        public NetworkInviteDomainFactory(IUnitOfWork unitOfWork, INetworkInviteRepository<INetworkInviteModel, INetworkInviteDomainFactory> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public INetworkInviteModel BuildNetworkInviteModel()
        {
            return new NetworkInviteModel(_unitOfWork, _repository);
        }
    }
}
