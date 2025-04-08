﻿using exSales.Domain.Impl.Factory;
using exSales.Domain.Impl.Models;
using exSales.Domain.Interfaces.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Models
{
    public interface IUserAddressModel
    {
        long Id { get; set; }
        long UserId { get; set; }
        ChainEnum Chain { get; set; }
        DateTime CreateAt { get; set; }
        DateTime UpdateAt { get; set; }
        string Address {  get; set; }
        IUserAddressModel Insert();
        IUserAddressModel Update();

        void Remove(long addressId);
        IEnumerable<IUserAddressModel> ListByUser(long userId, IUserAddressDomainFactory factory);
        IUserAddressModel GetById(long id, IUserAddressDomainFactory factory);
        IUserAddressModel GetByChain(long userId, ChainEnum chain, IUserAddressDomainFactory factory);

    }
}
