using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Timers;
namespace LocationSimulator_WPF
{
    public abstract class NavigationSensorBase : INavigationSensor
    {
        private System.Timers.Timer _timer;
        private LocationData _lastReading;
        private int _intervalMs = 100;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract string SensorName { get; }

        public int IntervalMs
        {
            get => _intervalMs;
            set
            {
                if (value > 0 && value != _intervalMs)
                {
                    _intervalMs = value;
                    _timer.Interval = _intervalMs;
                    OnPropertyChanged(nameof(IntervalMs));
                }
            }
        }

        public LocationData LastReading
        {
            get => _lastReading;
            protected set
            {
                _lastReading = value;
           //     OnPropertyChanged(nameof(LastReading)); // for ????
                OnPropertyChanged(nameof(LastReadingText)); // for UI
            }
        }

        public string LastReadingText => _lastReading?.ToString() ?? "Not started";

        public bool IsRunning => _timer.Enabled;

        // מימוש INavigationSensor - אירוע
        public event Action<LocationData> ReadingAvailable;

        public NavigationSensorBase()
        {
            _timer = new System.Timers.Timer(IntervalMs);
            _timer.Elapsed += Timer_Elapsed; // חיבור האירוע של הטיימר ללוגיקה
        }

        protected abstract  LocationData GenerateNewReading();

        // טיפול בטיימר: הקריאה מתבצעת ומופצת באירוע
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 1. קבלת קריאה חדשה
            LocationData newReading = GenerateNewReading();

            // 2. שמירה ועדכון ה-UI
            LastReading = newReading;

            // 3. הפצת האירוע למאזינים (NavigationService וכו')
            ReadingAvailable?.Invoke(newReading);
        }

        // מימוש INavigationSensor - מתודות

        public void Start()
        {
            _timer.Start();
            OnPropertyChanged(nameof(IsRunning));
        }

        public void Stop()
        {
            _timer.Stop();
            OnPropertyChanged(nameof(IsRunning));
        }
    }
}
