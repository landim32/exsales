using System;
using exSales.DTO.Domain;

namespace exSales.DTO.User
{
    public class UserResult : StatusResult
    {
        public UserInfo User { get; set; }
    }
}
