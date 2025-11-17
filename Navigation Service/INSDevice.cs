using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigation_Service
{
    internal class INSDevice : INavigationDevice
    {
        // constructor
        public INSDevice() { }

        // implement the event from the interface
        public event EventHandler<PositionArrivedEventArgs> onPositionArrived;
        public void SendPosition()
        {
            Position pos = new Position
            {
                x = new Random().NextDouble() * 100,
                y = new Random().NextDouble() * 100,
                z = new Random().NextDouble() * 100
            };

            PositionArrivedEventArgs args = new PositionArrivedEventArgs(pos);

            onPositionArrived?.Invoke(this, args);
        }
    }
}
