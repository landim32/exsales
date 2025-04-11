using Core.Domain.Repository;
using DB.Infra.Context;
using exSales.Domain.Interfaces.Factory;
using exSales.Domain.Interfaces.Models;
using exSales.DTO.Invoice;
using exSales.DTO.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Infra.Repository
{
    public class NetworkRepository : INetworkRepository<INetworkModel, INetworkDomainFactory>
    {
        private ExSalesContext _ccsContext;

        public NetworkRepository(ExSalesContext ccsContext)
        {
            _ccsContext = ccsContext;
        }

        private INetworkModel DbToModel(INetworkDomainFactory factory, Network row)
        {
            var md = factory.BuildNetworkModel();
            md.NetworkId = row.NetworkId;
            md.Name = row.Name;
            md.Email = row.Email;
            md.Commission = row.Commission;
            md.WithdrawalMin = row.WithdrawalMin;
            md.WithdrawalPeriod = row.WithdrawalPeriod;
            md.Status = (NetworkStatusEnum) row.Status;
            return md;
        }

        private void ModelToDb(INetworkModel md, Network row)
        {
            row.NetworkId = md.NetworkId;
            row.Name = md.Name;
            row.Email = md.Email;
            row.Commission = md.Commission;
            row.WithdrawalMin = md.WithdrawalMin;
            row.WithdrawalPeriod = md.WithdrawalPeriod;
            row.Status = (int)md.Status;
        }

        public INetworkModel Insert(INetworkModel model, INetworkDomainFactory factory)
        {
            var row = new Network();
            ModelToDb(model, row);
            _ccsContext.Add(row);
            _ccsContext.SaveChanges();
            model.NetworkId = row.NetworkId;
            return model;
        }

        public INetworkModel Update(INetworkModel model, INetworkDomainFactory factory)
        {
            var row = _ccsContext.Networks.Find(model.NetworkId);
            ModelToDb(model, row);
            _ccsContext.Networks.Update(row);
            _ccsContext.SaveChanges();
            return model;
        }

        public IEnumerable<INetworkModel> ListAll(INetworkDomainFactory factory)
        {
            return _ccsContext.Networks.Select(x => DbToModel(factory, x));
        }

        public INetworkModel GetById(long id, INetworkDomainFactory factory)
        {
            var row = _ccsContext.Networks.Find(id);
            if (row == null)
                return null;
            return DbToModel(factory, row);
        }
    }
}
