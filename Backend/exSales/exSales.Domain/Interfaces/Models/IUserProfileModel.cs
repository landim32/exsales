using exSales.Domain.Interfaces.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Models
{
    public interface IUserProfileModel
    {
        long ProfileId { get; set; }

        long NetworkId { get; set; }

        string Name { get; set; }

        int Commission { get; set; }

        IEnumerable<IUserProfileModel> ListByNetwork(long networkId, IUserProfileDomainFactory factory);
        IUserProfileModel Insert(IUserProfileModel model, IUserProfileDomainFactory factory);
        IUserProfileModel Update(IUserProfileModel model, IUserProfileDomainFactory factory);
    }
}
