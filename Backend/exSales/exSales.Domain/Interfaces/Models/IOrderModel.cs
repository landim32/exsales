using exSales.Domain.Interfaces.Factory;
using exSales.DTO.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Models
{
    public interface IOrderModel
    {
        long OrderId { get; set; }

        long ProductId { get; set; }

        long UserId { get; set; }

        OrderStatusEnum Status { get; set; }

        IEnumerable<IOrderModel> ListByUser(long networkId, long userId, IOrderDomainFactory factory);
        IOrderModel GetById(long id, IOrderDomainFactory factory);
        IOrderModel Insert(IOrderModel model, IOrderDomainFactory factory);
        IOrderModel Update(IOrderModel model, IOrderDomainFactory factory);
    }
}
