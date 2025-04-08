﻿using System;
using System.Collections.Generic;

namespace Core.Domain.Repository
{
    public interface IUserRepository<TModel, TFactory>
    {
        TModel Insert(TModel model, TFactory factory);
        TModel Update(TModel model, TFactory factory);
        IEnumerable<TModel> ListUsers(TFactory factory);
        TModel GetById(long userId, TFactory factory);
        TModel GetByEmail(string email, TFactory factory);
        TModel GetByAddress(int chainId, string address, TFactory factory);
        TModel GetByToken(string token, TFactory factory);
        void UpdateToken(long userId, string token);
        TModel LoginWithEmail(string email, string encryptPwd, TFactory factory);
        TModel GetUserByRecoveryHash(string recoveryHash, TFactory factory);
        void UpdateRecoveryHash(long userId, string recoveryHash);
        void ChangePassword(long userId, string encryptPwd);
        bool HasPassword(long userId, TFactory factory);
    }
}
