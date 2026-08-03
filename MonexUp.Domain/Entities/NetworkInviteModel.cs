using Core.Domain;
using Core.Domain.Repository;
using MonexUp.Domain.Interfaces.Factory;
using MonexUp.Domain.Interfaces.Models;
using MonexUp.DTO.Network;
using System;
using System.Collections.Generic;

namespace MonexUp.Domain.Impl.Models
{
    public class NetworkInviteModel : INetworkInviteModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INetworkInviteRepository<INetworkInviteModel, INetworkInviteDomainFactory> _repository;

        public NetworkInviteModel(IUnitOfWork unitOfWork, INetworkInviteRepository<INetworkInviteModel, INetworkInviteDomainFactory> repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public long InviteId { get; set; }
        public long NetworkId { get; set; }
        public string Email { get; set; }
        public long InviterUserId { get; set; }
        public NetworkInviteStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConsumedAt { get; set; }
        public long? ConsumedUserId { get; set; }

        public INetworkInviteModel Insert(INetworkInviteDomainFactory factory)
        {
            return _repository.Insert(this, factory);
        }

        public INetworkInviteModel Update(INetworkInviteDomainFactory factory)
        {
            return _repository.Update(this, factory);
        }

        public INetworkInviteModel GetById(long inviteId, INetworkInviteDomainFactory factory)
        {
            return _repository.GetById(inviteId, factory);
        }

        public INetworkInviteModel GetPending(long networkId, string email, INetworkInviteDomainFactory factory)
        {
            return _repository.GetPending(networkId, email, factory);
        }

        public IList<INetworkInviteModel> ListPendingByNetwork(long networkId, INetworkInviteDomainFactory factory)
        {
            return _repository.ListPendingByNetwork(networkId, factory);
        }
    }
}
