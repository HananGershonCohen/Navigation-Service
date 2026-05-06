using MathNet.Numerics.LinearAlgebra;

namespace Navigation_Service
{
    public class NavigationState
    {
        public DateTime Timestamp { get; set; }
        public double Latitude { get; set; } = 0.0;
        public double Longitude { get; set; } = 0.0;
        public double Altitude { get; set; } = 0.0;
        public double Roll { get; set; } = 0.0;
        public double Pitch { get; set; } = 0.0;
        public double Yaw { get; set; } = 0.0;
        public double SpeedMs { get; set; } = 0.0;

        // Indicates if the navigation state is ready for use (e.g., after initialization)
        public bool IsReady { get; set; } = false;

        public void UpdateFromKalmanState(Vector<double> stateVector)
        {
            this.Latitude = stateVector[0];
            this.Longitude = stateVector[1];
            this.Altitude = stateVector[2];
            this.SpeedMs = stateVector[3];
            this.Roll = stateVector[4];
            this.Pitch = stateVector[5];
            this.Yaw = stateVector[6];
        }
    }


        

    }