using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel; // for INotifyPropertyChanged

namespace LocationSimulator_WPF
{
    internal interface INavigationSensor : INotifyPropertyChanged
    {
        string SensorName { get; }
        int IntervalMs { get; set; }
        LocationData LastReading { get; }
        bool IsRunning { get; }
        void Start();
        void Stop();

        event Action<LocationData> ReadingAvailable;
    }
}
