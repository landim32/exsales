﻿using exSales.Domain.Interfaces.Factory;
using exSales.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Models
{
    public interface IUserNetworkModel
    {
        long UserId { get; set; }

        long NetworkId { get; set; }

        long ProfileId { get; set; }

        UserRoleEnum Role { get; set; }

        UserNetworkStatusEnum Status { get; set; }

        long? ReferrerId { get; set; }

        IEnumerable<IUserNetworkModel> ListByUser(long userId, IUserNetworkDomainFactory factory);
        IUserNetworkModel Insert(IUserNetworkModel model, IUserNetworkDomainFactory factory);
        IUserNetworkModel Update(IUserNetworkModel model, IUserNetworkDomainFactory factory);
    }
}
