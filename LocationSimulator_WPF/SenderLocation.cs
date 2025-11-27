using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationSimulator_WPF
{
    public class SenderLocation
    {
        public SenderLocation()
        {
            //addSensor();
            //registerForEvent();
        }

        private void addSensor()
        {
            navigationSensors.Add(new GpsSensor());
        }

        public void RegisterSensor(INavigationSensor sensor)
        {
            // Ensure the sensor instance is not null
            if (sensor != null)
            {
                // Register the handler to the sensor instance provided by the Controller
                sensor.OnReadingAvailable += Sensor_ReadingAvailable;
            }
        }
        private void registerForEvent()
        {
            foreach (var sensor in navigationSensors)
            {
                sensor.OnReadingAvailable += Sensor_ReadingAvailable;


            }
        }

        private void Sensor_ReadingAvailable(object sender, ReadingArrivedEventArgs e)
        {
            // send position to server;
            // protocol UDP
           Debug.WriteLine("ReadingAvailable !! i need to send Location to NavogationService ");
        }

        private void openSocketandSend()
        {
            // open UDP socket
        }

        // properties
        private List<INavigationSensor> navigationSensors = new List<INavigationSensor>();
        int IP; 
        int port;

    }
}
