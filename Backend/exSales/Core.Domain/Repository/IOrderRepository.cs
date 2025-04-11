﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Repository
{
    public interface IOrderRepository<TModel, TFactory>
    {
        IEnumerable<TModel> ListByUser(long networkId, long userId, TFactory factory);
        TModel GetById(long id, TFactory factory);
        TModel Insert(TModel model, TFactory factory);
        TModel Update(TModel model, TFactory factory);
    }
}
