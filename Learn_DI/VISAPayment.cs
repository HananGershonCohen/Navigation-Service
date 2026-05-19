using System;
using System.Collections.Generic;
using System.Text;

namespace Learn_DI
{
    internal class VISAPayment : IPaymentService
    {
        public void ProcessPayment(decimal amount)
        {
            // Simulate processing payment with a credit card
            Console.WriteLine("VISA Payment");
        }
    }
}
