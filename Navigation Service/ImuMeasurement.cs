using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigation_Service
{
    public class ImuMeasurement : IMeasurement // Inherits from the common interface we created
    {
        // --- Time ---

        // Delta Time (dt) in seconds. Used for calculation.
        // The formula: dt = CurrentTimestamp - PreviousTimestamp
        public double Timestamp { get; set; }

        // --- Accelerometers ---

        // (Meters per second squared) m/s^2 units: usually
        // Role in filter: advances the Position and Speed
        public double AccelX { get; set; } // Longitudinal acceleration (Forward/Backward)
        public double AccelY { get; set; } // Lateral acceleration (Right/Left)
        public double AccelZ { get; set; } // Vertical acceleration (Including gravity!)

        // --- Gyroscopes ---

        // (Radians per second) Rad/s units: usually
        // Role in filter: advances the angles (Roll, Pitch, Yaw)
        public double GyroX { get; set; } // Roll Rate
        public double GyroY { get; set; } // Pitch Rate
        public double GyroZ { get; set; } // Yaw Rate
    }
}
