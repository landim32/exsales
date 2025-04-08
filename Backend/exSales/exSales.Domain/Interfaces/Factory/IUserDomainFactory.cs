using System;
using exSales.Domain.Interfaces.Models;

namespace exSales.Domain.Interfaces.Factory
{
    public interface IUserDomainFactory
    {
        IUserModel BuildUserModel();
    }
}
