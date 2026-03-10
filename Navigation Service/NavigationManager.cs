using Serilog;
using Serilog.Core;

namespace Navigation_Service
{
    internal class NavigationManager
    {

        private readonly ILogger _logger;
        private readonly List<INavigationDevice> _navigationDevices;
        private readonly NavigationState _currentState;
        public NavigationManager(ILogger logger,List<INavigationDevice> devices ) 
        {
             _logger = logger.ForContext<NavigationManager>();
            _navigationDevices = devices;
            _currentState = new NavigationState();

            foreach (var device in _navigationDevices)
            {
                device.onPositionArrived += HandleMeasurementReceived; // subscribe to all devices' events
            }

        }

        private void HandleMeasurementReceived(object sender, PositionArrivedEventArgs e)
        {
           
            _logger.Information("[NavigationManager]HandleMeasurementReceived");
        }

        public void run()
        {
        //    updateUdpReceiversAndDevices();

            while (true)
            {
                // keep the service running
                Task.Delay(1000).Wait();
                // if user press enter , break 

            }
        }

    }
}
