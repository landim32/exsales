using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Repository
{
    public interface INetworkRepository<TModel, TFactory>
    {
        IEnumerable<TModel> ListAll(TFactory factory);
        TModel GetById(long id, TFactory factory);
        TModel Insert(TModel model, TFactory factory);
        TModel Update(TModel model, TFactory factory);
    }
}
