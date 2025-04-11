using Core.Domain.Repository;
using Core.Domain;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Impl.Models
{
    public class UserProfileModel : IUserProfileModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProfileRepository<IUserProfileModel, IUserProfileDomainFactory> _repositoryProfile;

        public UserProfileModel(IUnitOfWork unitOfWork, IUserProfileRepository<IUserProfileModel, IUserProfileDomainFactory> repositoryProfile)
        {
            _unitOfWork = unitOfWork;
            _repositoryProfile = repositoryProfile;
        }

        public long ProfileId { get; set; }
        public long NetworkId { get; set; }
        public string Name { get; set; }
        public int Commission { get; set; }

        public IUserProfileModel Insert(IUserProfileModel model, IUserProfileDomainFactory factory)
        {
            return _repositoryProfile.Insert(model, factory);
        }

        public IEnumerable<IUserProfileModel> ListByNetwork(long networkId, IUserProfileDomainFactory factory)
        {
            return _repositoryProfile.ListByNetwork(networkId, factory);
        }

        public IUserProfileModel Update(IUserProfileModel model, IUserProfileDomainFactory factory)
        {
            return _repositoryProfile.Update(model, factory);
        }
    }
}
