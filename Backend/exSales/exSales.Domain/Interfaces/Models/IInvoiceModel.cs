﻿using exSales.Domain.Interfaces.Factory;
using exSales.DTO.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Models
{
    public interface IInvoiceModel
    {
        long InvoiceId { get; set; }

        long OrderId { get; set; }

        long UserId { get; set; }

        long? SellerId { get; set; }

        double Price { get; set; }

        DateTime DueDate { get; set; }

        DateTime? PaymentDate { get; set; }

        InvoiceStatusEnum Status { get; set; }

        IInvoiceModel Insert(IInvoiceDomainFactory factory);
        IInvoiceModel Update(IInvoiceDomainFactory factory);

        IEnumerable<IInvoiceModel> ListByUser(long networkId, long userId, IInvoiceDomainFactory factory);
        IInvoiceModel GetById(long id, IInvoiceDomainFactory factory);
    }
}
