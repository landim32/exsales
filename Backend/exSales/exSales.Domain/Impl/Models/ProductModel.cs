﻿using Core.Domain.Repository;
using Core.Domain;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using exSales.DTO.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Impl.Models
{
    public class ProductModel : IProductModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository<IProductModel, IProductDomainFactory> _repositoryProduct;

        public ProductModel(IUnitOfWork unitOfWork, IProductRepository<IProductModel, IProductDomainFactory> repositoryProduct)
        {
            _unitOfWork = unitOfWork;
            _repositoryProduct = repositoryProduct;
        }

        public long ProductId { get; set; }
        public long NetworkId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Frequency { get; set; }
        public int Limit { get; set; }
        public ProductStatusEnum Status { get; set; }

        public IProductModel GetById(long id, IProductDomainFactory factory)
        {
            return _repositoryProduct.GetById(id, factory);
        }

        public IProductModel Insert(IProductModel model, IProductDomainFactory factory)
        {
            return _repositoryProduct.Insert(model, factory);
        }

        public IEnumerable<IProductModel> ListByNetwork(long networkId, IProductDomainFactory factory)
        {
            return _repositoryProduct.ListByNetwork(networkId, factory);
        }

        public IProductModel Update(IProductModel model, IProductDomainFactory factory)
        {
            return _repositoryProduct.Update(model, factory);
        }
    }
}
