using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationSimulator_WPF
{
    internal class GpsSensor  : NavigationSensorBase
    {
        public override string SensorName => "GPS Sensor";

        protected override LocationData GenerateNewReading()
        {
            // לוגיקה פשוטה לדוגמה ליצירת קריאת מיקום אקראית
            double x = 1.0;
            double y = 1.0;
            double z = 1.0;
            double roll = 1.0;
            double pitch = 1.0;
            double yaw = 1.0;
            return new LocationData
            {
                SensorName = SensorName,
                Timestamp = DateTime.Now,
                X = x,
                Y = y,
                Z = z,
                Roll = roll,
                Pitch = pitch,
                Yaw = yaw
            };
        }
    }
}
