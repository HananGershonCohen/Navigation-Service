using System;
using System.Collections.Generic;
using System.Text;

namespace Learn_DI
{
    internal class OrderManager
    {
        private readonly INotificationService _notificationService;
        private readonly IPaymentService _paymentService;

        //(Constructor Injection)
        public OrderManager(INotificationService notificationService, IPaymentService paymentService)
        {
            _notificationService = notificationService;
            _paymentService = paymentService;
        }

        public void Checkout(decimal price)
        {
            _paymentService.ProcessPayment(price);
            _notificationService.SendNotification($"Thank you!");
        
        }
    }
}
