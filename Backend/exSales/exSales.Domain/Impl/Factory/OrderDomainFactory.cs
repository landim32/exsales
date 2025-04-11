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
    public class OrderDomainFactory : IOrderDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository<IOrderModel, IOrderDomainFactory> _repositoryOrder;

        public OrderDomainFactory(IUnitOfWork unitOfWork, IOrderRepository<IOrderModel, IOrderDomainFactory> repositoryOrder)
        {
            _unitOfWork = unitOfWork;
            _repositoryOrder = repositoryOrder;
        }

        public IOrderModel BuildOrderModel()
        {
            return new OrderModel(_unitOfWork, _repositoryOrder);
        }
    }
}
