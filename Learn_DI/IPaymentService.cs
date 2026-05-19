using System;
using System.Collections.Generic;
using System.Text;

namespace Learn_DI
{
    internal interface IPaymentService
    {
            void ProcessPayment(decimal amount);
    }
}
