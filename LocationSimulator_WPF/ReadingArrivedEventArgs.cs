using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationSimulator_WPF
{
    public class ReadingArrivedEventArgs  : EventArgs
    {
        public  LocationData ReadingData { get; private set; }
        public ReadingArrivedEventArgs(LocationData reading)
        {
            ReadingData = reading;
        }
    }
}
