using System;
using System.Collections.Generic;
using System.Text;

namespace Learn_DI
{
    internal interface INotificationService
    {
            void SendNotification(string message);
    }
}
