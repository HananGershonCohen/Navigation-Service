using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigation_Service
{
    internal class NavigationManager
    {
        public NavigationManager() { }

        // add device to the list
        public void AddDevice()
        {
            navigationDevices.Add(new GPSDevice());
            navigationDevices.Add(new INSDevice());
            navigationDevices.Add(new CameraDevice());
            Console.WriteLine("Devices Added");
        }

        private void Device_onPositionArrived(object sender, PositionArrivedEventArgs e)
        {
            // Logic to handle the new position from any device
            // We access the Position object via e.PositionData
            double posX = e.PositionData.x;
            double posY = e.PositionData.y;

            e.PositionData.z = 10;

            double posZ = e.PositionData.z;


            // Print the device name and the X and Y coordinates with 2 decimal places (F2)
            Console.WriteLine($"Manager received position from {sender.GetType().Name}: X={posX}, Y={posY} , {posZ}");
        }

        private void registerForEvent()
        {
            // Iterate over all devices and subscribe to their event
            foreach (var device in navigationDevices)
            {
                // This subscribes the 'Device_onPositionArrived' method to the event of each device
                device.onPositionArrived += Device_onPositionArrived;
            }
            Console.WriteLine("Manager registered for all device events.");
        }

        public void run()
        {
           AddDevice();
            registerForEvent();

            System.Console.WriteLine("Navigation Manager is running. Press Enter to stop and exit...");
            Console.ReadLine();
        }
        

        private List<INavigationDevice> navigationDevices = new List<INavigationDevice>();


        

    }
}
