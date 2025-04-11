﻿using System;
using System.Collections.Generic;
using System.Linq;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using Core.Domain.Repository;
using DB.Infra.Context;
using System.Net;

namespace DB.Infra.Repository
{
    public class UserRepository : IUserRepository<IUserModel, IUserDomainFactory>
    {

        private ExSalesContext _ccsContext;

        public UserRepository(ExSalesContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private IUserModel DbToModel(IUserDomainFactory factory, User u)
        {
            var md = factory.BuildUserModel();
            md.Id = u.UserId;
            md.Hash = u.Hash;
            md.Name = u.Name;
            md.Email = u.Email;
            md.IsAdmin = u.IsAdmin;
            md.CreatedAt = u.CreatedAt;
            md.UpdatedAt = u.UpdatedAt;
            return md;
        }

        private void ModelToDb(IUserModel md, User row)
        {
            row.UserId = md.Id;
            row.Hash = md.Hash;
            row.Name = md.Name;
            row.Email = md.Email;
            row.IsAdmin = md.IsAdmin;
            row.CreatedAt = md.CreatedAt;
            row.UpdatedAt = md.UpdatedAt;
        }

        public IUserModel GetById(long userId, IUserDomainFactory factory)
        {
            var row = _ccsContext.Users.Find(userId);
            if (row == null)
                return null;
            return DbToModel(factory, row);
        }

        public IUserModel Update(IUserModel model, IUserDomainFactory factory)
        {
            var row = _ccsContext.Users.Where(x => x.UserId == model.Id).FirstOrDefault();
            ModelToDb(model, row);
            row.UpdatedAt = DateTime.Now;
            _ccsContext.Users.Update(row);
            _ccsContext.SaveChanges();
            return model;
        }


        public IUserModel Insert(IUserModel model, IUserDomainFactory factory)
        {
            var u = new User();
            ModelToDb(model, u);
            u.CreatedAt = DateTime.Now;
            u.UpdatedAt = DateTime.Now;
            _ccsContext.Add(u);
            _ccsContext.SaveChanges();
            model.Id = u.UserId;
            return model;
        }

        public IEnumerable<IUserModel> ListUsers(IUserDomainFactory factory)
        {
            var rows = _ccsContext.Users.ToList();
            return rows.Select(x => DbToModel(factory, x));
        }

        public IUserModel GetByEmail(string email, IUserDomainFactory factory)
        {
            var row = _ccsContext.Users.Where(x => x.Email == email).FirstOrDefault();
            if (row != null)
            {
                return DbToModel(factory, row);
            }
            return null;
        }

        public IUserModel GetByToken(string token, IUserDomainFactory factory)
        {
            var row = _ccsContext.Users.Where(x => x.Token == token).FirstOrDefault();
            if (row != null)
            {
                return DbToModel(factory, row);
            }
            return null;
        }

        public void UpdateToken(long userId, string token) {
            var row = _ccsContext.Users.Find(userId);
            row.Token = token;
            _ccsContext.Users.Update(row);
            _ccsContext.SaveChanges();
        }

        public IUserModel LoginWithEmail(string email, string encryptPwd, IUserDomainFactory factory)
        {
            var row = _ccsContext.Users
                .Where(x => x.Email == email.ToLower() && x.Password == encryptPwd)
                .FirstOrDefault();
            if (row != null)
            {
                return DbToModel(factory, row);
            }
            return null;
        }

        public bool HasPassword(long userId, IUserDomainFactory factory)
        {
            var row = _ccsContext.Users.Find(userId);
            return row != null && !string.IsNullOrEmpty(row.Password);
        }

        public IUserModel GetUserByRecoveryHash(string recoveryHash, IUserDomainFactory factory)
        {
            var row = _ccsContext.Users
                .Where(x => x.RecoveryHash == recoveryHash)
                .FirstOrDefault();
            if (row != null)
            {
                return DbToModel(factory, row);
            }
            return null;
        }

        public void UpdateRecoveryHash(long userId, string recoveryHash)
        {
            var row = _ccsContext.Users.Find(userId);
            row.UpdatedAt = DateTime.Now;
            row.RecoveryHash = recoveryHash;
            _ccsContext.Users.Update(row);
            _ccsContext.SaveChanges();
        }

        public void ChangePassword(long userId, string encryptPwd)
        {
            var row = _ccsContext.Users.Find(userId);
            row.UpdatedAt = DateTime.Now;
            row.Password = encryptPwd;
            _ccsContext.Users.Update(row);
            _ccsContext.SaveChanges();
        }
    }
}
