//using MathNet.Numerics.LinearAlgebra;
//using Serilog;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Navigation_Service
//{
//    internal class NavigationManager
//    {
//        public enum NavigationStatus
//        {
//            Idle,               // System started, waiting for first data
//            WaitingForAnchor,   // We received INS but missing global position to start
//            Ready               // System initialized and navigating
//        }

//        private readonly ILogger _logger;
//        private readonly List<INavigationDevice> _navigationDevices;
//        private readonly NavigationState _CurrentState;
//        private readonly LocationSender _locationSender;
//        private NavigationStatus _status = NavigationStatus.Idle;
//        private readonly KalmanFilterEngine _kalmanFilter;
//        private readonly Matrix<double> _transitionMatrixF;
//        private readonly Vector<double> _measurementVectorZ;
//        private DateTime _lastTimestamp;

//        public NavigationManager(ILogger logger, List<INavigationDevice> devices)
//        {
//            _logger = logger.ForContext<NavigationManager>();
//            _navigationDevices = devices;
//            _CurrentState = new NavigationState();
//            _locationSender = new LocationSender(Constants.SIMULATOR_PORT, _CurrentState, logger);

//            // Initialize Kalman Filter
//            var initialState = Vector<double>.Build.Dense(7); // All zeros
//            var initialCovariance = Matrix<double>.Build.DenseIdentity(7) * 0.1;
//            var processNoise = Matrix<double>.Build.DenseIdentity(7) * 0.01;
//            var measurementNoise = Matrix<double>.Build.DenseIdentity(3) * 5.0;
//            var measurementMatrix = Matrix<double>.Build.Dense(3, 7);
//            measurementMatrix[0, 0] = 1; // Map Latitude
//            measurementMatrix[1, 1] = 1; // Map Longitude
//            measurementMatrix[2, 2] = 1; // Map Altitude

//            _kalmanFilter = new KalmanFilterEngine(initialState, initialCovariance, processNoise, measurementNoise, measurementMatrix);

//            // Initialize reusable matrices
//            _transitionMatrixF = Matrix<double>.Build.DenseIdentity(7);
//            _measurementVectorZ = Vector<double>.Build.Dense(7);

//            foreach (var device in _navigationDevices)
//            {
//                device.onPositionArrived += HandleMeasurementReceived; // subscribe to all devices' events
//            }
//        }

//        private async void HandleMeasurementReceived(object sender, PositionArrivedEventArgs e)
//        {
//            _logger.Information("[NavigationManager] HandleMeasurementReceived");
//            IMeasurement measurement = e._position;

//            TryInitialize(measurement);

//            // if not ready after trying, not process with filter.
//            if (_status != NavigationStatus.Ready) return;

//            // Process the measurement with the Kalman filter
//            ProcessMeasurementWithFilter(measurement);

//            await _locationSender.SendCurrentStateAsync();
//        }

//        private void TryInitialize(IMeasurement measurement)
//        {
//            // If the system is already ready, no need to initialize again
//            if (_status == NavigationStatus.Ready) return;

//            if (measurement is IGlobalPositionSource posSource)
//            {
//                // Initialize the system with GPS data
//                _CurrentState.Latitude = posSource.Latitude;
//                _CurrentState.Longitude = posSource.Longitude;
//                _CurrentState.Altitude = posSource.Altitude;
//                _CurrentState.Timestamp = measurement.Timestamp;

//                if (measurement is IGlobalVelocitySource velSource)
//                {
//                    _CurrentState.SpeedMs = velSource.SpeedMs;
//                    _CurrentState.Yaw = velSource.CourseRad;
//                }

//                _status = NavigationStatus.Ready;
//                _CurrentState.IsReady = true;
//                _logger.Information("[Init] Navigation system READY. Initialized via {SourceType}", measurement.GetType().Name);
//            }
//            else
//            {
//                // If no GPS data is available, move to WaitingForAnchor
//                _status = NavigationStatus.WaitingForAnchor;
//                _logger.Warning("[Init] Waiting for GPS to initialize...");
//            }
//        }

//        private void UpdateMatrix_F(double deltaTime)
//        {
//            _transitionMatrixF[0, 3] = deltaTime; // x position depends on x velocity
//            _transitionMatrixF[1, 4] = deltaTime; // y position depends on y velocity
//            _transitionMatrixF[2, 5] = deltaTime; // z position depends on z velocity
//        }

//        private void UpdateVector_Z()
//        {
//            _measurementVectorZ[0] = _CurrentState.Latitude;
//            _measurementVectorZ[1] = _CurrentState.Longitude;
//            _measurementVectorZ[2] = _CurrentState.Altitude;
//            _measurementVectorZ[3] = _CurrentState.SpeedMs; // Assuming speed is along the direction of travel
//            _measurementVectorZ[4] = _CurrentState.Roll;
//            _measurementVectorZ[5] = _CurrentState.Pitch;
//            _measurementVectorZ[6] = _CurrentState.Yaw;
//        }


//        private void ProcessMeasurementWithFilter(IMeasurement measurement)
//        {
//            double deltaTime = measurement.Timestamp.Second - _lastTimestamp.Second;
//            _lastTimestamp = measurement.Timestamp;

//            // Update transition matrix F
//            UpdateMatrix_F(deltaTime);

//            if (measurement is IInertialMeasurementSource imu)
//            {
//                _kalmanFilter.Predict(_transitionMatrixF);
//            }
//            else if (measurement is IGlobalPositionSource gps)
//            {
//                // Update measurement vector Z
//                UpdateVector_Z();
//                _kalmanFilter.Update(_measurementVectorZ);
//            }

//            // Update _CurrentState with Kalman Filter results
//            _CurrentState.UpdateFromKalmanState(_kalmanFilter.GetState());
//        }

//        public void run()
//        {
//            // Start the LocationSender
//            while (true)
//            {
//                // keep the service running
//                Task.Delay(1000).Wait();
//                // if user press enter , break
//            }
//        }
//    }
//}


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

        // ווקטור המדידה חייב להיות במימד 3 כדי להתאים למטריצה H שהגדרת (3x7)
        private readonly Vector<double> _measurementVectorZ3;
        private DateTime _lastTimestamp;

        public NavigationManager(ILogger logger, List<INavigationDevice> devices)
        {
            _logger = logger.ForContext<NavigationManager>();
            _navigationDevices = devices;
            _CurrentState = new NavigationState();
            _locationSender = new LocationSender(Constants.SIMULATOR_PORT, _CurrentState, logger);

            var initialState = Vector<double>.Build.Dense(7);
            var initialCovariance = Matrix<double>.Build.DenseIdentity(7) * 0.1;
            var processNoise = Matrix<double>.Build.DenseIdentity(7) * 0.01;

            // רעש המדידה תואם למימד 3 (Lat, Lon, Alt)
            var measurementNoise = Matrix<double>.Build.DenseIdentity(3) * 0.00001;
            var measurementMatrix = Matrix<double>.Build.Dense(3, 7);
            measurementMatrix[0, 0] = 1;
            measurementMatrix[1, 1] = 1;
            measurementMatrix[2, 2] = 1;

            _kalmanFilter = new KalmanFilterEngine(initialState, initialCovariance, processNoise, measurementNoise, measurementMatrix);

            _transitionMatrixF = Matrix<double>.Build.DenseIdentity(7);

            // אתחול הווקטור במימד 3
            _measurementVectorZ3 = Vector<double>.Build.Dense(3);

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
                _lastTimestamp = measurement.Timestamp; // חשוב לאתחול זמן הייחוס


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
            // מחיקת ערכים קודמים כדי למנוע הצטברות שגיאות
            _transitionMatrixF.Clear();
            for (int i = 0; i < 7; i++) _transitionMatrixF[i, i] = 1.0; // אלכסון של 1

            // 1 מעלת קו רוחב (Latitude) שווה בערך ל-111,139 מטרים
            double metersToDegreesLat = 1.0 / 111139.0;

            // 1 מעלת קו אורך (Longitude) משתנה בהתאם למיקום על הכדור
            double currentLatRad = _CurrentState.Latitude * (Math.PI / 180.0);
            double metersToDegreesLon = 1.0 / (111139.0 * Math.Cos(currentLatRad));

            // פירוק המהירות הכוללת לרכיבי צפון ומזרח בעזרת זווית ה-Yaw (כיוון ההתקדמות)
            // הערה: נניח ש-Yaw=0 זה צפון
            double speedNorth = _CurrentState.SpeedMs * Math.Cos(_CurrentState.Yaw);
            double speedEast = _CurrentState.SpeedMs * Math.Sin(_CurrentState.Yaw);

            // הוספת ה"מרחק" שעברנו במעלות למטריצת התחזית
            // מאחר והמהירות אינה מיוצגת כרכיבי X/Y נפרדים בוקטור המצב, 
            // נכניס את ההשפעה ישירות כאן בצורה מקורבת:

            // (הערה: פילטר קלמן לינארי מתקשה פה כי Yaw משתנה, אבל זה ימנע את הקפיצות הענקיות!)
            _transitionMatrixF[0, 3] = Math.Cos(_CurrentState.Yaw) * deltaTime * metersToDegreesLat; // Lat depends on Speed & Yaw
            _transitionMatrixF[1, 3] = Math.Sin(_CurrentState.Yaw) * deltaTime * metersToDegreesLon; // Lon depends on Speed & Yaw

            // כרגע אין לנו רכיב מהירות אנכית בוקטור ה-State (אינדקס 3 הוא מהירות אופקית כוללת).
            // לכן נשאיר את הגובה (Alt - אינדקס 2) ללא שינוי ממודל התנועה, והוא יסתמך רק על מדידות ה-GPS/Barometer.
        }

        // הפונקציה המתוקנת מקבלת את מקור המידע ומעדכנת רק את מה שנמדד (מימד 3)
        private void UpdateVector_Z(IGlobalPositionSource gps)
        {
            _measurementVectorZ3[0] = gps.Latitude;
            _measurementVectorZ3[1] = gps.Longitude;
            _measurementVectorZ3[2] = gps.Altitude;
        }

        private void ProcessMeasurementWithFilter(IMeasurement measurement)
        {
            // 1. חישוב הזמן שעבר
            double deltaTime = (measurement.Timestamp - _lastTimestamp).TotalSeconds;
            if (deltaTime < 0) deltaTime = 0;
            _lastTimestamp = measurement.Timestamp;

            // 2. עדכון מטריצת מודל התנועה (F)
            UpdateMatrix_F(deltaTime);

            // 3. טיפול בנתוני IMU (Inertial)
            if (measurement is IInertialMeasurementSource imu)
            {
                // עדכון ה-Yaw מהגירוסקופ (הוכחנו שזה עובד מצוין)
                var currentState = _kalmanFilter.GetState();
                double deltaYaw = imu.GyroZ * deltaTime;
                currentState[6] += deltaYaw;
                _kalmanFilter.SetState(currentState);

                _kalmanFilter.Predict(_transitionMatrixF);
            }
            // 4. טיפול בנתוני מיקום (GPS GGA)
            else if (measurement is IGlobalPositionSource gps)
            {
                UpdateVector_Z(gps);
                _kalmanFilter.Update(_measurementVectorZ3);
            }

            // 5. טיפול בנתוני מהירות וכיוון (GPS VTG) - >>> כאן התיקון הקריטי! <<<
            if (measurement is IGlobalVelocitySource velSource)
            {
                _CurrentState.SpeedMs = velSource.SpeedMs;
                _CurrentState.Yaw = velSource.CourseRad;

                var currentState = _kalmanFilter.GetState();
                currentState[3] = velSource.SpeedMs;    // עדכון המהירות בתוך המצב של קלמן!
                currentState[6] = velSource.CourseRad;  // עדכון כיוון הטיסה
                _kalmanFilter.SetState(currentState);
            }

            // 6. עדכון האובייקט הכללי מהפילטר והדפסה ללוג
            _CurrentState.UpdateFromKalmanState(_kalmanFilter.GetState());

            var innov = _kalmanFilter.LastInnovation;
            _logger.Information("[Kalman] Innovation (Lat/Lon/Alt): {LatInnov:F8}, {LonInnov:F8}, {AltInnov:F2}",
                innov[0], innov[1], innov[2]);

            _logger.Information("[NS - Kalman] Updated state: Lat={Latitude}, Lon={Longitude}, Alt={Altitude}, Speed={SpeedMs}, Yaw={Yaw}",
                _CurrentState.Latitude, _CurrentState.Longitude, _CurrentState.Altitude, _CurrentState.SpeedMs, _CurrentState.Yaw);

            _CurrentState.Timestamp = measurement.Timestamp;
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

