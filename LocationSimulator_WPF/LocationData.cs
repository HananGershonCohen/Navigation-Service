using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationSimulator_WPF
{
    /// <summary>
    /// Represents a full pose with 6 degrees of freedom.
    /// </summary>
    public class LocationData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Roll { get; set; }
        public int Pitch { get; set; }
        public double Yaw { get; set; }
        public string SensorName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public override string ToString()
        {
            return $"[{SensorName}] X:{X:F2} Y:{Y:F2} Z:{Z:F2} | R:{Roll:F2} P:{Pitch:F2} Y:{Yaw:F2}";
        }
    }
}
