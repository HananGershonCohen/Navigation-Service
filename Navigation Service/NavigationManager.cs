using MathNet.Numerics.LinearAlgebra;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Navigation_Service
{
    internal class NavigationManager
    {
        public enum NavigationStatus
        {
            Idle,
            WaitingForAnchor,
            Ready
        }

        private readonly ILogger _logger;
        private readonly List<INavigationDevice> _navigationDevices;
        private readonly NavigationState _CurrentState;
        private readonly LocationSender _locationSender;
        private NavigationStatus _status = NavigationStatus.Idle;
        private readonly KalmanFilterEngine _kalmanFilter;

        private readonly Matrix<double> _transitionMatrixF;
        private readonly Matrix<double> _measurementMatrixH3;
        private readonly Matrix<double> _measurementMatrixH4;
        private readonly Matrix<double> _measurementNoiseR3;
        private readonly Matrix<double> _measurementNoiseR4;

        private DateTime _lastTimestamp;

        public NavigationManager(ILogger logger, List<INavigationDevice> devices)
        {
            _logger = logger.ForContext<NavigationManager>();
            _navigationDevices = devices;
            _CurrentState = new NavigationState();
            _locationSender = new LocationSender(Constants.SIMULATOR_PORT, _CurrentState, logger);

            // וקטור מצב: [Lat, Lon, Alt, Speed, Roll, Pitch, Yaw]
            var initialState = Vector<double>.Build.Dense(7);
            var initialCovariance = Matrix<double>.Build.DenseIdentity(7) * 0.1;
            var processNoise = Matrix<double>.Build.DenseIdentity(7) * 0.01;

            // כיול רעשי מדידה לקנה מידה של מעלות עשרוניות בריבוע
            _measurementNoiseR3 = Matrix<double>.Build.DenseIdentity(3) * 0.0000001;
            _measurementNoiseR4 = Matrix<double>.Build.DenseIdentity(4) * 0.0000001;
            _measurementNoiseR4[3, 3] = 0.2; // שגיאת מהירות m/s בריבוע

            // מטריצת מיקום בלבד (GGA)
            _measurementMatrixH3 = Matrix<double>.Build.Dense(3, 7);
            _measurementMatrixH3[0, 0] = 1; // Lat
            _measurementMatrixH3[1, 1] = 1; // Lon
            _measurementMatrixH3[2, 2] = 1; // Alt

            // מטריצת מיקום + מהירות (VTG / Combined)
            _measurementMatrixH4 = Matrix<double>.Build.Dense(4, 7);
            _measurementMatrixH4[0, 0] = 1; // Lat
            _measurementMatrixH4[1, 1] = 1; // Lon
            _measurementMatrixH4[2, 2] = 1; // Alt
            _measurementMatrixH4[3, 3] = 1; // SpeedMs

            _kalmanFilter = new KalmanFilterEngine(initialState, initialCovariance, processNoise, _measurementNoiseR3, _measurementMatrixH3);
            _transitionMatrixF = Matrix<double>.Build.DenseIdentity(7);

            foreach (var device in _navigationDevices)
            {
                device.onPositionArrived += HandleMeasurementReceived;
            }
        }

        private async void HandleMeasurementReceived(object sender, PositionArrivedEventArgs e)
        {
            IMeasurement measurement = e._position;
            TryInitialize(measurement);

            if (_status != NavigationStatus.Ready) return;

            ProcessMeasurementWithFilter(measurement);
            await _locationSender.SendCurrentStateAsync();
        }

        private void TryInitialize(IMeasurement measurement)
        {
            if (_status == NavigationStatus.Ready) return;

            if (measurement is IGlobalPositionSource posSource)
            {
                _CurrentState.Latitude = posSource.Latitude;
                _CurrentState.Longitude = posSource.Longitude;
                _CurrentState.Altitude = posSource.Altitude;
                _CurrentState.Timestamp = measurement.Timestamp;
                _lastTimestamp = measurement.Timestamp;

                var initialVec = Vector<double>.Build.Dense(7);
                initialVec[0] = posSource.Latitude;
                initialVec[1] = posSource.Longitude;
                initialVec[2] = posSource.Altitude;

                if (measurement is IGlobalVelocitySource velSource)
                {
                    _CurrentState.SpeedMs = velSource.SpeedMs;
                    _CurrentState.Yaw = velSource.CourseRad;
                    initialVec[3] = velSource.SpeedMs;
                    initialVec[6] = velSource.CourseRad;
                }

                _kalmanFilter.SetState(initialVec);
                _status = NavigationStatus.Ready;
                _CurrentState.IsReady = true;
                _logger.Information("[Init] Navigation system READY.");
            }
        }

        private void UpdateMatrix_F(double deltaTime)
        {
            _transitionMatrixF.Clear();
            for (int i = 0; i < 7; i++) _transitionMatrixF[i, i] = 1.0;

            double metersToDegreesLat = 1.0 / 111139.0;
            double currentLatRad = _CurrentState.Latitude * (Math.PI / 180.0);
            double metersToDegreesLon = 1.0 / (111139.0 * Math.Cos(currentLatRad));

            var state = _kalmanFilter.GetState();
            double currentYaw = state[6];

            _transitionMatrixF[0, 3] = Math.Cos(currentYaw) * deltaTime * metersToDegreesLat;
            _transitionMatrixF[1, 3] = Math.Sin(currentYaw) * deltaTime * metersToDegreesLon;
        }

        private void ProcessMeasurementWithFilter(IMeasurement measurement)
        {
            double deltaTime = (measurement.Timestamp - _lastTimestamp).TotalSeconds;
            if (deltaTime < 0) deltaTime = 0;
            _lastTimestamp = measurement.Timestamp;

            double gyroZ = measurement is IInertialMeasurementSource imu ? imu.GyroZ : 0;

            UpdateMatrix_F(deltaTime);

            // 1. שלב ה-Predict (ריצה על נתוני אינרציאלים IMU)
            if (measurement is IInertialMeasurementSource)
            {
                if (gyroZ != 0)
                {
                    // שילוב קצב הסיבוב בתוך ה-State רגע לפני ה-Predict לצורך שמירה על ה-Covariance
                    var state = _kalmanFilter.GetState();
                    state[6] += gyroZ * deltaTime;
                    _kalmanFilter.SetState(state);
                }
                _kalmanFilter.Predict(_transitionMatrixF);
            }
            // 2. שלב ה-Update (ריצה על נתוני GPS חיצוניים)
            else if (measurement is IGlobalPositionSource gps)
            {
                _kalmanFilter.Predict(_transitionMatrixF);

                if (measurement is IGlobalVelocitySource velSource)
                {
                    // מקרה 1: הגיע מידע משולב הכולל מהירות (VTG) -> מעבר בטוח למימד 4 ללא Reflection
                    _kalmanFilter.UpdateMeasurementModels(_measurementMatrixH4, _measurementNoiseR4);

                    var z4 = Vector<double>.Build.DenseOfArray(new double[] {
                        gps.Latitude, gps.Longitude, gps.Altitude, velSource.SpeedMs
                    });
                    _kalmanFilter.Update(z4);

                    // תיקון רציפות הצידוד על בסיס הלוויינים
                    var state = _kalmanFilter.GetState();
                    state[6] = velSource.CourseRad;
                    _kalmanFilter.SetState(state);
                }
                else
                {
                    // מקרה 2: הגיעה מדידת מיקום בלבד (GGA) -> מעבר בטוח למימד 3 ללא פולבקים דורסים
                    _kalmanFilter.UpdateMeasurementModels(_measurementMatrixH3, _measurementNoiseR3);

                    var z3 = Vector<double>.Build.DenseOfArray(new double[] {
                        gps.Latitude, gps.Longitude, gps.Altitude
                    });
                    _kalmanFilter.Update(z3);
                }
            }

            // 3. סנכרון המצב הסופי לעדכון ה-UI
            var currentKalmanState = _kalmanFilter.GetState();
            _CurrentState.UpdateFromKalmanState(currentKalmanState);
            _CurrentState.Timestamp = measurement.Timestamp;

            var innov = _kalmanFilter.LastInnovation;
            if (innov != null)
            {
                _logger.Debug("[Kalman] Innovation Size: {Size}, LatInnov: {LatInnov:F8}", innov.Count, innov[0]);
            }
        }

        public void run()
        {
            while (true)
            {
                Task.Delay(1000).Wait();
            }
        }
    }
}