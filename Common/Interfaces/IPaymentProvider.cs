using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IPaymentProvider
    {
        public Task CreatePayment();
        public Task GetPaymentStatus();
    }
}
