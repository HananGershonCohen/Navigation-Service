using Serilog;
using Serilog.Core;

namespace Navigation_Service
{
    internal class NavigationManager
    {
        private readonly ILogger _logger;
        public NavigationManager() {
            _logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .CreateLogger()
                    .ForContext<NavigationManager>();

            _logger.Information("NavigationManager created and Logger initialized.");
        }
        private void updateUdpReceiversAndDevices()
        {
            // 1. start udp listener on GNSS port
            UdpNmeaSource gnssUdpSource = new UdpNmeaSource(Constants.GNSS_PORT);

            // 2. create GNSS device
            GNSSDevice gnssDevice = new GNSSDevice();

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
