using MonexUp.Domain.Interfaces.Factory;
using MonexUp.DTO.Network;
using System;
using System.Collections.Generic;

namespace MonexUp.Domain.Interfaces.Models
{
    public interface INetworkInviteModel
    {
        long InviteId { get; set; }
        long NetworkId { get; set; }
        string Email { get; set; }
        long InviterUserId { get; set; }
        NetworkInviteStatusEnum Status { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? ConsumedAt { get; set; }
        long? ConsumedUserId { get; set; }

        INetworkInviteModel Insert(INetworkInviteDomainFactory factory);
        INetworkInviteModel Update(INetworkInviteDomainFactory factory);
        INetworkInviteModel GetById(long inviteId, INetworkInviteDomainFactory factory);
        INetworkInviteModel GetPending(long networkId, string email, INetworkInviteDomainFactory factory);
        IList<INetworkInviteModel> ListPendingByNetwork(long networkId, INetworkInviteDomainFactory factory);
    }
}
