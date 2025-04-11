using Core.Domain.Repository;
using DB.Infra.Context;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using exSales.DTO.Order;
using exSales.DTO.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infra.Repository
{
    public class ProductRepository : IProductRepository<IProductModel, IProductDomainFactory>
    {
        private ExSalesContext _ccsContext;

        public ProductRepository(ExSalesContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private IProductModel DbToModel(IProductDomainFactory factory, Product row)
        {
            var md = factory.BuildProductModel();
            md.ProductId = row.ProductId;
            md.NetworkId = row.NetworkId;
            md.Name = row.Name;
            md.Price = row.Price;
            md.Frequency = row.Frequency;
            md.Limit = row.Limit;
            md.Status = (ProductStatusEnum) row.Status;
            return md;
        }

        private void ModelToDb(IProductModel md, Product row)
        {
            row.ProductId = md.ProductId;
            row.NetworkId = md.NetworkId;
            row.Name = md.Name;
            row.Price = md.Price;
            row.Frequency = md.Frequency;
            row.Limit = md.Limit;
            row.Status = (int)md.Status;
        }

        public IProductModel Insert(IProductModel model, IProductDomainFactory factory)
        {
            var row = new Product();
            ModelToDb(model, row);
            _ccsContext.Add(row);
            _ccsContext.SaveChanges();
            model.ProductId = row.ProductId;
            return model;
        }

        public IProductModel Update(IProductModel model, IProductDomainFactory factory)
        {
            var row = _ccsContext.Products.Find(model.ProductId);
            ModelToDb(model, row);
            _ccsContext.Products.Update(row);
            _ccsContext.SaveChanges();
            return model;
        }

        public IEnumerable<IProductModel> ListByNetwork(long networkId, IProductDomainFactory factory)
        {
            return _ccsContext.Products
                .Where(x => x.NetworkId == networkId)
                .Select(x => DbToModel(factory, x));
        }

        public IProductModel GetById(long id, IProductDomainFactory factory)
        {
            var row = _ccsContext.Products.Find(id);
            if (row == null)
                return null;
            return DbToModel(factory, row);
        }
    }
}
