using Core.Domain.Repository;
using Core.Domain;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Impl.Models
{
    public class UserPhoneModel : IUserPhoneModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory> _repositoryPhone;

        public UserPhoneModel(IUnitOfWork unitOfWork, IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory> repositoryPhone)
        {
            _unitOfWork = unitOfWork;
            _repositoryPhone = repositoryPhone;
        }

        public long PhoneId { get; set; }
        public long UserId { get; set; }
        public string Phone { get; set; }

        public void Delete(long phoneId)
        {
            _repositoryPhone.Delete(phoneId);
        }

        public IUserPhoneModel Insert(IUserPhoneModel model, IUserPhoneDomainFactory factory)
        {
            return _repositoryPhone.Insert(model, factory);
        }

        public IEnumerable<IUserPhoneModel> ListByUser(long userId, IUserPhoneDomainFactory factory)
        {
            return _repositoryPhone.ListByUser(userId, factory);
        }

        public IUserPhoneModel Update(IUserPhoneModel model, IUserPhoneDomainFactory factory)
        {
            return _repositoryPhone.Update(model, factory);
        }
    }
}
