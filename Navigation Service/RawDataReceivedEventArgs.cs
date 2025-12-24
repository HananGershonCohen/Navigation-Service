using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigation_Service
{
    public class RawDataReceivedEventArgs : EventArgs
    {
        public byte[] RawData { get; private set; }
        public RawDataReceivedEventArgs(byte[] rawData)
        {
            RawData = rawData;
        }
    }
}
