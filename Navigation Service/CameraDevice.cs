using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigation_Service
{
    internal class CameraDevice : INavigationDevice
    {
        // constructor
        public CameraDevice() { }

        // implement the event from the interface
        public event EventHandler<PositionArrivedEventArgs> onPositionArrived;

        // send Event of position arrived
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
