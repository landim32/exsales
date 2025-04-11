using Core.Domain.Repository;
using Core.Domain;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exSales.Domain.Impl.Models;

namespace exSales.Domain.Impl.Factory
{
    public class UserAddressDomainFactory : IUserAddressDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory> _repositoryAddr;

        public UserAddressDomainFactory(IUnitOfWork unitOfWork, IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory> repositoryAddr)
        {
            _unitOfWork = unitOfWork;
            _repositoryAddr = repositoryAddr;
        }
        public IUserAddressModel BuildUserAddressModel()
        {
            return new UserAddressModel(_unitOfWork, _repositoryAddr);
        }
    }
}
