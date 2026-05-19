using System;
using System.Collections.Generic;
using System.Text;

namespace Learn_DI
{
    internal class PayBox : IPaymentService
    {
        public void ProcessPayment(decimal amount)
        {
            Console.WriteLine("PayBox Paymaent");
        }
    }
}
