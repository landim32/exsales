using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exSales.DTO.Invoice
{
    public enum InvoiceStatusEnum
    {
        Open = 1,
        Paid = 2,
        Cancelled = 3,
        Refunded = 4
    }
}
