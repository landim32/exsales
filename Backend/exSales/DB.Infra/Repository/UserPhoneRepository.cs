using Core.Domain.Repository;
using DB.Infra.Context;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using exSales.DTO.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infra.Repository
{
    public class UserPhoneRepository : IUserPhoneRepository<IUserPhoneModel, IUserPhoneDomainFactory>
    {
        private ExSalesContext _ccsContext;

        public UserPhoneRepository(ExSalesContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private IUserPhoneModel DbToModel(IUserPhoneDomainFactory factory, UserPhone row)
        {
            var md = factory.BuildUserPhoneModel();
            md.PhoneId = row.PhoneId;
            md.UserId = row.UserId;
            md.Phone = row.Phone;
            return md;
        }

        private void ModelToDb(IUserPhoneModel md, UserPhone row)
        {
            row.PhoneId = md.PhoneId;
            row.UserId = md.UserId;
            row.Phone = md.Phone;
        }

        public IUserPhoneModel Insert(IUserPhoneModel model, IUserPhoneDomainFactory factory)
        {
            var row = new UserPhone();
            ModelToDb(model, row);
            _ccsContext.Add(row);
            _ccsContext.SaveChanges();
            model.PhoneId = row.PhoneId;
            return model;
        }

        public IUserPhoneModel Update(IUserPhoneModel model, IUserPhoneDomainFactory factory)
        {
            var row = _ccsContext.UserPhones.Find(model.PhoneId);
            ModelToDb(model, row);
            _ccsContext.UserPhones.Update(row);
            _ccsContext.SaveChanges();
            return model;
        }

        public void Delete(long phoneId)
        {
            var row = _ccsContext.UserPhones.Find(phoneId);
            if (row == null)
                return;
            _ccsContext.UserPhones.Remove(row);
            _ccsContext.SaveChanges();
        }

        public IEnumerable<IUserPhoneModel> ListByUser(long userId, IUserPhoneDomainFactory factory)
        {
            return _ccsContext.UserPhones
                .Where(x => x.UserId == userId)
                .Select(x => DbToModel(factory, x));
        }
    }
}
