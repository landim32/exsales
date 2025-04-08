﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Repository
{
    public interface IUserAddressRepository<TModel, TFactory>
    {
        IEnumerable<TModel> ListByUser(long userId, TFactory factory);
        TModel GetById(long addressId, TFactory factory);
        TModel GetByChain(long userId, int ChainId, TFactory factory);
        TModel Insert(TModel model);
        TModel Update(TModel model);
        void Delete(long id);
    }
}
