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
    public class ProductDomainFactory : IProductDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository<IProductModel, IProductDomainFactory> _repositoryProduct;

        public ProductDomainFactory(IUnitOfWork unitOfWork, IProductRepository<IProductModel, IProductDomainFactory> repositoryProduct)
        {
            _unitOfWork = unitOfWork;
            _repositoryProduct = repositoryProduct;
        }

        public IProductModel BuildProductModel()
        {
            return new ProductModel(_unitOfWork, _repositoryProduct);
        }
    }
}
