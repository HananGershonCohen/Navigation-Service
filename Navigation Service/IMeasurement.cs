using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigation_Service
{
    public interface IMeasurement
    {
        // The timestamp is critical for synchronization between them
        double Timestamp { get; }
    }
}
