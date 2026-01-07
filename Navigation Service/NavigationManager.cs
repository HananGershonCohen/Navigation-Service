using Serilog;
using Serilog.Core;

namespace Navigation_Service
{
    internal class NavigationManager
    {

        private readonly ILogger _logger;
        private readonly List<INavigationDevice> _navigationDevices;
        public NavigationManager(ILogger logger,List<INavigationDevice> device ) 
        {
             _logger = logger.ForContext<NavigationManager>();
            _navigationDevices = device;
        }
        //private void updateUdpReceiversAndDevices()
        //{
        //    // 1. start udp listener on GNSS port
        //    UdpNmeaSource gnssUdpSource = new UdpNmeaSource(Constants.GNSS_PORT,_logger);

        //    // 2. create GNSS device
        //    GNSSDevice gnssDevice = new GNSSDevice(_logger);

        //    // 3. connect GNSS device to UDP source
        //    gnssDevice.ConnectSource(gnssUdpSource);

        //    // 4. add to lists
        //    navigationDevices.Add(gnssDevice);
        //    nmeaSources.Add(gnssUdpSource);
        //}

        public void run()
        {
        //    updateUdpReceiversAndDevices();

            while (true)
            {
                // keep the service running
                Task.Delay(1000).Wait();
            }
        }

    }
}
