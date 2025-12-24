using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Navigation_Service
{
    internal class NavigationManager
    {
        public NavigationManager() { }
        private void updateUdpReceiversAndDevices()
        {
            // There needs to be one object that does all this work. It looks really ugly to have to manually add every sensor to the vector.
            UdpReceiver GpsReceiver = new UdpReceiver(Constants.GPS_PORT);
            GPSDevice gpsDevice = new GPSDevice();
            GpsReceiver.RawDataReceived += gpsDevice.On_RawDataReceived;
            navigationDevices.Add(gpsDevice);
            udpReceivers.Add(GpsReceiver);


        }

        private void StartListening()
        {
            foreach (var udpReceiver in udpReceivers)
            {
                // Start listening asynchronously for each UDP receiver
                Task.Run(() => udpReceiver.StartListening());
            }
            Console.WriteLine("UDP Receivers started listening.");
        }
        public void run()
        {
            ////////AddDevice();
            //////// registerForEvent();
            updateUdpReceiversAndDevices();
            StartListening();

            System.Console.WriteLine("Navigation Manager is running. Press Enter to stop and exit...");
            Console.ReadLine();
        }
        

        private List<INavigationDevice> navigationDevices = new List<INavigationDevice>();
        private List<UdpReceiver> udpReceivers = new List<UdpReceiver>();


    }
}
