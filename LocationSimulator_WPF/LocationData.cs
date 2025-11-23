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
        // מיקום (Transalation)
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        // אוריינטציה (Rotation)
        public double Roll { get; set; }
        public double Pitch { get; set; }
        public double Yaw { get; set; }

        // זיהוי וזמן
        public string SensorName { get; init; } // init מאפשר הגדרה רק בבנייה
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"[{SensorName}] X:{X:F2} Y:{Y:F2} Z:{Z:F2} | R:{Roll:F2} P:{Pitch:F2} Y:{Yaw:F2}";
        }
    }
}
