﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Repository
{
    public interface IProductRepository<TModel, TFactory>
    {
        IEnumerable<TModel> ListByNetwork(long networkId, TFactory factory);
        TModel GetById(long id, TFactory factory);
        TModel Insert(TModel model, TFactory factory);
        TModel Update(TModel model, TFactory factory);
    }
}
