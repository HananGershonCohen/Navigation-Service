using Serilog;
using Serilog.Core;

namespace Navigation_Service
{
    internal class NavigationManager
    {

        private readonly ILogger _logger;
        private readonly List<INavigationDevice> _navigationDevices;
        private readonly NavigationState _currentState;
        private readonly LocationSender _locationSender;

        private bool _isInitializedState = false;
        public NavigationManager(ILogger logger,List<INavigationDevice> devices ) 
        {
             _logger = logger.ForContext<NavigationManager>();
            _navigationDevices = devices;
            _currentState = new NavigationState();
            _locationSender = new LocationSender(Constants.SIMULATOR_PORT, _currentState, logger);
            foreach (var device in _navigationDevices)
            {
                device.onPositionArrived += HandleMeasurementReceived; // subscribe to all devices' events
            }

        }

        private void HandleMeasurementReceived(object sender, PositionArrivedEventArgs e)
        {
            _logger.Information("[NavigationManager]HandleMeasurementReceived");
            
            if (!_isInitializedState)
            {
                initState(e);
                return;
            }

            /*here we can implement the logic to update the navigation state based on the new measurement
             with Filter Kalman. */
            /*
                  if (e._position is ImuMeasurement imu) 
                  _kalmanFilter.Predict(imu, _currentState);
                    else if (e._position is GNSSPosition gps)
                  _kalmanFilter.Update(gps, _currentState);
             */
        }

        private void initState(PositionArrivedEventArgs e)
        {
            if (e._position is GNSSPosition gpsPosition)
            {
                //1. Mark the navigation state as initialized
                _isInitializedState = true;

                //2. Initialize the navigation state with the GPS position
                _currentState.Timestamp = gpsPosition.Timestamp;
                _currentState.Latitude = gpsPosition.Latitude;
                _currentState.Longitude = gpsPosition.Longitude;
                _currentState.Altitude = gpsPosition.Altitude;
                _currentState.SpeedMs = gpsPosition.SpeedMs;

                _logger.Information("[NavigationManager] Initialized state from GPS: Timestamp={Timestamp}, Latitude={Latitude}, Longitude={Longitude}, Altitude={Altitude}, Speed={Speed}",
                    _currentState.Timestamp, _currentState.Latitude, _currentState.Longitude, _currentState.Altitude, _currentState.SpeedMs);
            }
        }

        public void run()
        {

            // Start the LocationSender
            _locationSender.Start();

            while (true)
            {
                // keep the service running
                Task.Delay(1000).Wait();
                // if user press enter , break 

            }
        }

    }
}
