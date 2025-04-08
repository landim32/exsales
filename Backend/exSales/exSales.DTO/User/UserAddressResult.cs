using exSales.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.DTO.User
{
    public class UserAddressResult: StatusResult
    {
        public UserAddressInfo UserAddress { get; set; }
    }
}
