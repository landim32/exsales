using exSales.DTO.MailerSend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.Domain.Interfaces.Services
{
    public interface IMailerSendService
    {
        Task<bool> Sendmail(MailerInfo email);
    }
}
