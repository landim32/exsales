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
    public class UserDocumentDomainFactory : IUserDocumentDomainFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserDocumentRepository<IUserDocumentModel, IUserDocumentDomainFactory> _repositoryDocument;

        public UserDocumentDomainFactory(IUnitOfWork unitOfWork, IUserDocumentRepository<IUserDocumentModel, IUserDocumentDomainFactory> repositoryDocument)
        {
            _unitOfWork = unitOfWork;
            _repositoryDocument = repositoryDocument;
        }

        public IUserDocumentModel BuildUserDocumentModel()
        {
            return new UserDocumentModel(_unitOfWork, _repositoryDocument);
        }
    }
}
