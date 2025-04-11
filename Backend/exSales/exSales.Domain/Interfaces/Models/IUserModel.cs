﻿using exSales.Domain.Impl.Models;
using exSales.Domain.Interfaces.Factory;
using System;
using System.Collections.Generic;

namespace exSales.Domain.Interfaces.Models
{
    public interface IUserModel
    {
        long Id { get; set; }
        string Hash { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string IdDocument { get; set; }
        string PixKey { get; set; }
        DateTime? BirthDate { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        bool IsAdmin { get; set; }

        IUserModel Save(IUserDomainFactory factory);
        IUserModel Update(IUserDomainFactory factory);
        IUserModel GetByEmail(string email, IUserDomainFactory factory);
        IUserModel GetById(long userId, IUserDomainFactory factory);
        IUserModel GetByToken(string token, IUserDomainFactory factory);
        string GenerateNewToken(IUserDomainFactory factory);
        IUserModel GetByRecoveryHash(string recoveryHash, IUserDomainFactory factory);
        IEnumerable<IUserModel> ListAllUsers(IUserDomainFactory factory);
        IUserModel LoginWithEmail(string email, string password, IUserDomainFactory factory);
        bool HasPassword(long userId, IUserDomainFactory factory);
        void ChangePassword(long userId, string password, IUserDomainFactory factory);
        string GenerateRecoveryHash(long userId, IUserDomainFactory factory);
    }
}
