using System;
using System.Collections.Generic;
using System.Text;

namespace Learn_DI
{
    internal class SmsNotificationService : INotificationService
    {
        public void SendNotification(string message)
        {
            Console.WriteLine($"SMS: {message}");
        }
    }
}
