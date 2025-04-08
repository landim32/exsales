using System;
using exSales.Domain.Impl.Models;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using Core.Domain;
using Core.Domain.Repository;

namespace exSales.Domain.Impl.Factory
{
    public class UserAddressDomainFactory : IUserAddressDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory> _repositoryUserAddr;

        public UserAddressDomainFactory(IUnitOfWork unitOfWork, IUserAddressRepository<IUserAddressModel, IUserAddressDomainFactory> repositoryUserAddr)
        {
            _unitOfWork = unitOfWork;
            _repositoryUserAddr = repositoryUserAddr;
        }

        public IUserAddressModel BuildUserAddressModel()
        {
            return new UserAddressModel(_unitOfWork, _repositoryUserAddr);
        }
    }
}
