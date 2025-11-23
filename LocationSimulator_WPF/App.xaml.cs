using System;
using System.Windows;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Configuration; // שמור על ה-using הזה אם הוא היה קיים
using System.Data;          // שמור על ה-using הזה אם הוא היה קיים


namespace LocationSimulator_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            // --- מתחיל בדיקת סימולטור חיישנים ---
            Debug.WriteLine("--- מתחיל בדיקת סימולטור חיישנים (פלט ל-Visual Studio Output Window) ---");

            // 1. יצירת מופע של החיישן היחיד (GPS)
            var gps = new GpsSensor { IntervalMs = 500 };

            // 2. רישום לוגיקת בדיקה לאירוע ReadingAvailable
            Action<LocationData> logData = (data) =>
            {
                Debug.WriteLine(data.ToString());
            };

            gps.ReadingAvailable += logData;

            // 3. התחלת הסימולציה
            Debug.WriteLine($"התחלת {gps.SensorName} בתדירות {gps.IntervalMs}ms");
            gps.Start();

            // *** בדיקה 5 שניות ***
            Task.Delay(5000).ContinueWith(_ =>
            {
                // 5. עצירת הסימולציה וסגירה
                gps.Stop();
                Debug.WriteLine("\n--- בדיקה הסתיימה. סוגר אפליקציה. ---");
                Application.Current.Shutdown();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            // בשלב הבא: new MainWindow().Show();
        }
    }
}