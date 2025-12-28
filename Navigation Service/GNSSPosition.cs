namespace Navigation_Service
{
    public class GNSSPosition
    {
        // --- Time ---
        // Time in seconds (used for dt calculation)
        public double Timestamp { get; set; }

        // --- Position (3D) ---
        // Latitude in degrees (from GGA)
        public double Latitude { get; set; }

        // Longitude in degrees (from GGA)
        public double Longitude { get; set; }

        // Altitude in meters (from GGA)
        public double Altitude { get; set; }

        // --- Velocity (2D/3D) ---
        // Velocity in m/s (from VTG/RMC)
        public double SpeedMs { get; set; }

        // Course in radians (from VTG/RMC)
        public double CourseRad { get; set; }

        // --- Uncertainty / Quality (Matrix R) ---
        // Horizontal error in meters (derived from HDOP)
        public double PosErrorH { get; set; }

        // Vertical error in meters (derived from VDOP/Estimation)
        public double PosErrorV { get; set; }

        // Indicates if the data is valid for processing
        public bool IsValid { get; set; }
    }
}