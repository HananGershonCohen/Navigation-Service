using System.Collections.ObjectModel;
using System.Linq;
using LocationSimulator_WPF;

/// <summary>
/// Main control unit, activates the UI
/// </summary>
public class SimulatorController
{
    public ObservableCollection<INavigationSensor> Sensors { get; } = new ObservableCollection<INavigationSensor>();

    public SimulatorController()
    {
        InitializeSensors();
    }

    private void InitializeSensors()
    {
        var gps = new GpsSensor { IntervalMs = 500 };
        Sensors.Add(gps);
    }

    public void StartAll()
    {
        foreach (var sensor in Sensors)
        {
            if (!sensor.IsRunning)
            {
                sensor.Start();
            }
        }
    }

    public void StopAll()
    {
        foreach (var sensor in Sensors)
        {
            if (sensor.IsRunning)
            {
                sensor.Stop();
            }
        }
    }
}