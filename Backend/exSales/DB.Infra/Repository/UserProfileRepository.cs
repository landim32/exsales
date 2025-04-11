﻿using Core.Domain.Repository;
using DB.Infra.Context;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infra.Repository
{
    public class UserProfileRepository : IUserProfileRepository<IUserProfileModel, IUserProfileDomainFactory>
    {
        private ExSalesContext _ccsContext;

        public UserProfileRepository(ExSalesContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private IUserProfileModel DbToModel(IUserProfileDomainFactory factory, UserProfile row)
        {
            var md = factory.BuildUserProfileModel();
            md.ProfileId = row.ProfileId;
            md.NetworkId = row.NetworkId;
            md.Name = row.Name;
            md.Commission = row.Commission;
            return md;
        }

        private void ModelToDb(IUserProfileModel md, UserProfile row)
        {
            row.ProfileId = md.ProfileId;
            row.NetworkId = md.NetworkId;
            row.Name = md.Name;
            row.Commission = md.Commission;
        }

        public IUserProfileModel Insert(IUserProfileModel model, IUserProfileDomainFactory factory)
        {
            var row = new UserProfile();
            ModelToDb(model, row);
            _ccsContext.Add(row);
            _ccsContext.SaveChanges();
            model.ProfileId = row.ProfileId;
            return model;
        }

        public IUserProfileModel Update(IUserProfileModel model, IUserProfileDomainFactory factory)
        {
            var row = _ccsContext.UserProfiles.Find(model.ProfileId);
            ModelToDb(model, row);
            _ccsContext.UserProfiles.Update(row);
            _ccsContext.SaveChanges();
            return model;
        }

        public IEnumerable<IUserProfileModel> ListByNetwork(long networkId, IUserProfileDomainFactory factory)
        {
            return _ccsContext.UserProfiles
                .Where(x => x.NetworkId == networkId)
                .Select(x => DbToModel(factory, x));
        }
    }
}
