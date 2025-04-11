﻿using Core.Domain.Repository;
using DB.Infra.Context;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using exSales.DTO.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infra.Repository
{
    public class InvoiceRepository : IInvoiceRepository<IInvoiceModel, IInvoiceDomainFactory>
    {
        private ExSalesContext _ccsContext;

        public InvoiceRepository(ExSalesContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private IInvoiceModel DbToModel(IInvoiceDomainFactory factory, Invoice row)
        {
            var md = factory.BuildInvoiceModel();
            md.InvoiceId = row.InvoiceId;
            md.OrderId = row.OrderId;
            md.UserId = row.UserId;
            md.SellerId = row.SellerId;
            md.Price = row.Price;
            md.DueDate = row.DueDate;
            md.PaymentDate = row.PaymentDate;
            md.Status = (InvoiceStatusEnum) row.Status;
            return md;
        }

        private void ModelToDb(IInvoiceModel md, Invoice row)
        {
            row.InvoiceId = md.InvoiceId;
            row.OrderId = md.OrderId;
            row.UserId = md.UserId;
            row.SellerId = md.SellerId;
            row.Price = md.Price;
            row.DueDate = md.DueDate;
            row.PaymentDate = md.PaymentDate;
            row.Status = (int) md.Status;
        }

        public IInvoiceModel Insert(IInvoiceModel model, IInvoiceDomainFactory factory)
        {
            var row = new Invoice();
            ModelToDb(model, row);
            _ccsContext.Add(row);
            _ccsContext.SaveChanges();
            model.InvoiceId = row.InvoiceId;
            return model;
        }

        public IInvoiceModel Update(IInvoiceModel model, IInvoiceDomainFactory factory)
        {
            var row = _ccsContext.Invoices.Find(model.InvoiceId);
            ModelToDb(model, row);
            _ccsContext.Invoices.Update(row);
            _ccsContext.SaveChanges();
            return model;
        }

        public IEnumerable<IInvoiceModel> ListByUser(long networkId, long userId, IInvoiceDomainFactory factory)
        {
            return _ccsContext.Invoices
                .Where(x => x.Order.Product.NetworkId == networkId && x.UserId == userId)
                .Select(x => DbToModel(factory, x));
        }

        public IInvoiceModel GetById(long id, IInvoiceDomainFactory factory)
        {
            var row = _ccsContext.Invoices.Find(id);
            if (row == null)
                return null;
            return DbToModel(factory, row);
        }
    }
}
