using System;
using System.Collections.Generic;
using System.Text;

namespace Learn_DI
{
    internal class GmailNotification : INotificationService
    {
        public void SendNotification(string message)
        {
            // Simulate sending an email notification
            Console.WriteLine($"Gmail Notification: {message}");
        }
    }
}
