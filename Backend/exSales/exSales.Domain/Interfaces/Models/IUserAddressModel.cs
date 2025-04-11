using exSales.Domain.Interfaces.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Models
{
    public interface IUserAddressModel
    {
        long AddressId { get; set; }

        public long UserId { get; set; }

        string ZipCode { get; set; }

        string Address { get; set; }

        string Complement { get; set; }

        string Neighborhood { get; set; }

        string City { get; set; }

        string State { get; set; }

        IEnumerable<IUserAddressModel> ListByUser(long userId, IUserAddressDomainFactory factory);
        IUserAddressModel Insert(IUserAddressModel model, IUserAddressDomainFactory factory);
        IUserAddressModel Update(IUserAddressModel model, IUserAddressDomainFactory factory);
        void Delete(long addressId);
    }
}
