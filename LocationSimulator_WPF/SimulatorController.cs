using System.Collections.ObjectModel;
using System.Linq;
using LocationSimulator_WPF;

/// <summary>
/// מנהל את אוסף החיישנים ומספק מתודות גלובליות לשליטה (עבור ה-UI).
/// </summary>
public class SimulatorController
{
    // משתנה שמחזיק את כל החיישנים - כרגע רק GPS
    // ObservableCollection נדרש ל-Data Binding דינמי ב-WPF.
    public ObservableCollection<INavigationSensor> Sensors { get; } = new ObservableCollection<INavigationSensor>();

    public SimulatorController()
    {
        InitializeSensors();
    }

    private void InitializeSensors()
    {
        // יצירת מופע של חיישן GPS בלבד
        var gps = new GpsSensor { IntervalMs = 500 };

        // הוספה לאוסף
        Sensors.Add(gps);

        // אין צורך ב-InsSensor או בחיישנים נוספים כרגע.
    }

    /// <summary>
    /// מתחיל את פעולת הניווט של כל החיישנים (Start All).
    /// </summary>
    public void StartAll()
    {
        // הפעלה של כל החיישנים באוסף (כרגע רק אחד)
        foreach (var sensor in Sensors)
        {
            if (!sensor.IsRunning)
            {
                sensor.Start();
            }
        }
    }

    /// <summary>
    /// עוצר את פעולת הניווט של כל החיישנים (Stop All).
    /// </summary>
    public void StopAll()
    {
        // עצירה של כל החיישנים באוסף
        foreach (var sensor in Sensors)
        {
            if (sensor.IsRunning)
            {
                sensor.Stop();
            }
        }
    }
}