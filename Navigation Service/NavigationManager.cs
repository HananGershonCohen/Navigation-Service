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
            nmeaSources.Add(gnssUdpSource);
        }

        public void run()
        {
            updateUdpReceiversAndDevices();
            while (true)
            {
                // keep the service running
                Task.Delay(1000).Wait();
            }
        }
        

        private List<INavigationDevice> navigationDevices = new List<INavigationDevice>();
        private List<INmeaSource> nmeaSources = new List<INmeaSource>();
    }
}
