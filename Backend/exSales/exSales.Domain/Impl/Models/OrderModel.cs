using Core.Domain.Repository;
using Core.Domain;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using exSales.DTO.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Impl.Models
{
    public class OrderModel : IOrderModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderRepository<IOrderModel, IOrderDomainFactory> _repositoryOrder;

        public OrderModel(IUnitOfWork unitOfWork, IOrderRepository<IOrderModel, IOrderDomainFactory> repositoryOrder)
        {
            _unitOfWork = unitOfWork;
            _repositoryOrder = repositoryOrder;
        }

        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public long UserId { get; set; }
        public OrderStatusEnum Status { get; set; }

        public IOrderModel Insert(IOrderModel model, IOrderDomainFactory factory)
        {
            return _repositoryOrder.Insert(model, factory);
        }

        public IOrderModel Update(IOrderModel model, IOrderDomainFactory factory)
        {
            return _repositoryOrder.Update(model, factory);
        }

        public IEnumerable<IOrderModel> ListByUser(long networkId, long userId, IOrderDomainFactory factory)
        {
            return _repositoryOrder.ListByUser(networkId, userId, factory);
        }

        public IOrderModel GetById(long id, IOrderDomainFactory factory)
        {
            return _repositoryOrder.GetById(id, factory);
        }
    }
}
