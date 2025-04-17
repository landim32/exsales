﻿using exSales.Domain.Interfaces.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Models
{
    public interface IUserPhoneModel
    {
        long PhoneId { get; set; }

        long UserId { get; set; }

        string Phone { get; set; }

        IEnumerable<IUserPhoneModel> ListByUser(long userId, IUserPhoneDomainFactory factory);
        IUserPhoneModel Insert(IUserPhoneDomainFactory factory);
        void DeleteAllByUser(long userId);
    }
}
