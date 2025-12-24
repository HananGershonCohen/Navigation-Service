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
            // 1. start udp listener on GNSS port
            UdpNmeaSource gnssUdpSource = new UdpNmeaSource(Constants.GNSS_PORT);

            // 2. create GNSS device
            GnssDevice gnssDevice = new GnssDevice();

            // 3. connect GNSS device to UDP source
            gnssDevice.ConnectSource(gnssUdpSource);

            // 4. add to lists
            navigationDevices.Add(gnssDevice);
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
            updateUdpReceiversAndDevices();
            StartListening();

            System.Console.WriteLine("Navigation Manager is running. Press Enter to stop and exit...");
            Console.ReadLine();
        }
        

        private List<INavigationDevice> navigationDevices = new List<INavigationDevice>();
        private List<UdpReceiver> udpReceivers = new List<UdpReceiver>();
    }
}
