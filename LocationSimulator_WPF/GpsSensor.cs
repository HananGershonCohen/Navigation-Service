using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationSimulator_WPF
{
    public class GpsSensor  : NavigationSensorBase
    {
        public override string SensorName => "GPS Sensor";

        ////protected override LocationData GenerateNewReading()
        ////{
        ////    // לוגיקה פשוטה לדוגמה ליצירת קריאת מיקום אקראית
        ////    double x = 1.0;
        ////    double y = 1.0;
        ////    double z = 1.0;
        ////    double roll = 1.0;
        ////    double pitch = 1.0;
        ////    double yaw = 1.0;
        ////    return new LocationData
        ////    {
        ////        SensorName = SensorName,
        ////        Timestamp = DateTime.Now,
        ////        X = x,
        ////        Y = y,
        ////        Z = z,
        ////        Roll = roll,
        ////        Pitch = pitch,
        ////        Yaw = yaw
        ////    };
        ////}
        private double _currentX = 30.0;
        private Random _random = new Random();
        protected override LocationData GenerateNewReading()
        {

            // מדמה תזוזה קטנה עם רעש קל (כ-5 מטר)
            _currentX += 0.5 + (_random.NextDouble() * 0.1);

            return new LocationData
            {
                SensorName = SensorName,
                Timestamp = DateTime.Now,
                X = _currentX,
                Y = 32.0 + (_random.NextDouble() * 0.001), // רעש קל
                Z = 10.0, // קבוע
                Roll = 0, // GPS אינו מספק אוריינטציה
                Pitch = 0,
                Yaw = 0
            };
        }
    }
}
