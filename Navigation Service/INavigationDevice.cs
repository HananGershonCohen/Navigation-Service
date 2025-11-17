using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigation_Service
{
    internal interface INavigationDevice
    {
        // every device must implement this event !
        event EventHandler<PositionArrivedEventArgs> onPositionArrived;

        void SendPosition();
    }
}
