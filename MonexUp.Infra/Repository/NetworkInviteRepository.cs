using Core.Domain.Repository;
using DB.Infra.Context;
using Microsoft.EntityFrameworkCore;
using MonexUp.Domain.Interfaces.Factory;
using MonexUp.Domain.Interfaces.Models;
using MonexUp.DTO.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DB.Infra.Repository
{
    public class NetworkInviteRepository : INetworkInviteRepository<INetworkInviteModel, INetworkInviteDomainFactory>
    {
        private readonly MonexUpContext _ccsContext;

        public NetworkInviteRepository(MonexUpContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private INetworkInviteModel DbToModel(INetworkInviteDomainFactory factory, NetworkInvite row)
        {
            var md = factory.BuildNetworkInviteModel();
            md.InviteId = row.InviteId;
            md.NetworkId = row.NetworkId;
            md.Email = row.Email;
            md.InviterUserId = row.InviterUserId;
            md.Status = (NetworkInviteStatusEnum)row.Status;
            md.CreatedAt = row.CreatedAt;
            md.ConsumedAt = row.ConsumedAt;
            md.ConsumedUserId = row.ConsumedUserId;
            return md;
        }

        private static void ModelToDb(INetworkInviteModel md, NetworkInvite row)
        {
            row.NetworkId = md.NetworkId;
            row.Email = Normalize(md.Email);
            row.InviterUserId = md.InviterUserId;
            row.Status = (int)md.Status;
            // consumed_at is `timestamp without time zone`; Npgsql refuses a Kind=Utc
            // DateTime for that type, so normalize whatever the caller handed us.
            row.ConsumedAt = md.ConsumedAt.HasValue
                ? DateTime.SpecifyKind(md.ConsumedAt.Value, DateTimeKind.Unspecified)
                : null;
            row.ConsumedUserId = md.ConsumedUserId;
        }

        private static string Normalize(string email)
            => email == null ? null : email.Trim().ToLowerInvariant();

        public INetworkInviteModel Insert(INetworkInviteModel model, INetworkInviteDomainFactory factory)
        {
            var row = new NetworkInvite
            {
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };
            ModelToDb(model, row);

            try
            {
                _ccsContext.NetworkInvites.Add(row);
                _ccsContext.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // Raced against another manager inviting the same e-mail — the
                // partial unique index rejected the duplicate. Return the winner.
                _ccsContext.Entry(row).State = EntityState.Detached;
                var raced = GetPendingRow(model.NetworkId, model.Email);
                if (raced != null)
                {
                    return DbToModel(factory, raced);
                }
                throw;
            }

            model.InviteId = row.InviteId;
            model.CreatedAt = row.CreatedAt;
            return model;
        }

        public INetworkInviteModel Update(INetworkInviteModel model, INetworkInviteDomainFactory factory)
        {
            var row = _ccsContext.NetworkInvites
                .FirstOrDefault(x => x.InviteId == model.InviteId);
            if (row == null)
                return null;
            ModelToDb(model, row);
            _ccsContext.NetworkInvites.Update(row);
            _ccsContext.SaveChanges();
            return model;
        }

        public INetworkInviteModel GetById(long inviteId, INetworkInviteDomainFactory factory)
        {
            var row = _ccsContext.NetworkInvites
                .AsNoTracking()
                .FirstOrDefault(x => x.InviteId == inviteId);
            return row == null ? null : DbToModel(factory, row);
        }

        public INetworkInviteModel GetPending(long networkId, string email, INetworkInviteDomainFactory factory)
        {
            var row = GetPendingRow(networkId, email);
            return row == null ? null : DbToModel(factory, row);
        }

        private NetworkInvite GetPendingRow(long networkId, string email)
        {
            var normalized = Normalize(email);
            return _ccsContext.NetworkInvites
                .AsNoTracking()
                .FirstOrDefault(x => x.NetworkId == networkId
                                     && x.Email == normalized
                                     && x.Status == (int)NetworkInviteStatusEnum.Pending);
        }

        public IList<INetworkInviteModel> ListPendingByNetwork(long networkId, INetworkInviteDomainFactory factory)
        {
            return _ccsContext.NetworkInvites
                .AsNoTracking()
                .Where(x => x.NetworkId == networkId
                            && x.Status == (int)NetworkInviteStatusEnum.Pending)
                .OrderByDescending(x => x.CreatedAt)
                .ToList()
                .Select(x => DbToModel(factory, x))
                .ToList();
        }
    }
}
