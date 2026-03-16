using Serilog;
using Serilog.Core;
using System.Diagnostics.Metrics;

namespace Navigation_Service
{
    internal class NavigationManager
    {
        public enum NavigationStatus
        {
            Idle,               // System started, waiting for first data
            WaitingForAnchor,   // We received INS but missing global position to start
            Ready               // System initialized and navigating
        }


        private readonly ILogger _logger;
        private readonly List<INavigationDevice> _navigationDevices;
        private readonly NavigationState _currentState;
        private readonly LocationSender _locationSender;
        private NavigationStatus _status = NavigationStatus.Idle;    
        // private bool _isInitializedState = false;
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

        private async void HandleMeasurementReceived(object sender, PositionArrivedEventArgs e)
        {
            _logger.Information("[NavigationManager]HandleMeasurementReceived");

            IMeasurement measurement = e._position;
            TryInitialize(measurement);
            await _locationSender.SendCurrentStateAsync();

            //if (_status != NavigationStatus.Ready)
            //{

            //    TryInitialize(measurement);
            //    // if not ready after trying, not process with filter.
            //    if (_status != NavigationStatus.Ready) return;
            //}

            //// Process the measurement with the Kalman filter
            //ProcessMeasurementWithFilter(measurement);

        }

        private void TryInitialize(IMeasurement measurement)
        {
            if (measurement is IGlobalPositionSource posSource)
            {
                _currentState.Latitude = posSource.Latitude;
                _currentState.Longitude = posSource.Longitude;
                _currentState.Altitude = posSource.Altitude;
                _currentState.Timestamp = measurement.Timestamp;

                if (measurement is IGlobalVelocitySource velSource)
                {
                    _currentState.SpeedMs = velSource.SpeedMs;
                    _currentState.Yaw = velSource.CourseRad;
                }

                _status = NavigationStatus.Ready;
                _currentState.IsReady = true; 
                _logger.Information("[Init] Navigation system READY. Initialized via {SourceType}", measurement.GetType().Name);
            }
            else if (_status == NavigationStatus.Idle)
            {
                _status = NavigationStatus.WaitingForAnchor;
                _logger.Warning("[Init] Received {SourceType}. Still waiting for Global Position (GPS) to initialize...", measurement.GetType().Name);
            }
        }

        private void ProcessMeasurementWithFilter(IMeasurement measurement)
        {
            _currentState.Timestamp = measurement.Timestamp;
            /*
            if (measurement is IInertialMeasurementSource imu)
                _kalmanFilter.Predict(imu, _currentState);
            else if (measurement is IGlobalPositionSource gps)
                _kalmanFilter.Update(gps, _currentState);
            */
        }

        public void run()
        {

            // Start the LocationSender

            while (true)
            {
                // keep the service running
                Task.Delay(1000).Wait();
                // if user press enter , break 

            }
        }

    }
}


