using System.Collections.Generic;

namespace Core.Domain.Repository
{
    public interface INetworkInviteRepository<TModel, TFactory>
    {
        TModel Insert(TModel model, TFactory factory);
        TModel Update(TModel model, TFactory factory);
        TModel GetById(long inviteId, TFactory factory);
        /// <summary>Pending invite for (networkId, email). E-mail is matched lowercased.</summary>
        TModel GetPending(long networkId, string email, TFactory factory);
        IList<TModel> ListPendingByNetwork(long networkId, TFactory factory);
    }
}
