﻿using System;
using System.Collections.Generic;
using exSales.Domain.Interfaces.Models;
using exSales.DTO.User;
using Microsoft.AspNetCore.Http;
using exSales.Domain.Impl.Models;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Services
{
    public interface IUserService
    {
        IUserModel LoginWithEmail(string email, string password);
        bool HasPassword(long userId);
        void ChangePasswordUsingHash(string recoveryHash, string newPassword);
        void ChangePassword(long userId, string oldPassword, string newPassword);
        Task<bool> SendRecoveryEmail(string email);

        IUserModel Insert(UserInfo user);
        IUserModel Update(UserInfo user);
        IUserModel GetUserByEmail(string email);
        IUserModel GetUserByAddress(ChainEnum chain, string address);
        IEnumerable<IUserModel> GetAllUserAddress();
        IUserModel GetUserByID(long userId);
        IUserModel GetUserByToken(string token);
        IUserModel GetUserHash(ChainEnum chain, string address);
        UserInfo GetUserInSession(HttpContext httpContext);

        IEnumerable<IUserAddressModel> ListAddressByUser(long userId);
        IUserAddressModel GetAddressByChain(long userId, ChainEnum chain);
        void AddOrChangeAddress(long userId, ChainEnum chain, string address);
        void RemoveAddress(long userId, ChainEnum chain);
    }
}
