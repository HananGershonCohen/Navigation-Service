using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigation_Service
{
    internal class NavigationDevicesBase : INavigationDevice
    {
        public NavigationDevicesBase() {
            startTimer();
        }

        private void startTimer()
        {
            System.Timers.Timer myTimer = new System.Timers.Timer(1000);
            myTimer.Elapsed += OnTimedEvent;
            myTimer.Enabled = true;
        }

        private void OnTimedEvent(object? sender, System.Timers.ElapsedEventArgs e)
        {
            SendPosition();
        }

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
