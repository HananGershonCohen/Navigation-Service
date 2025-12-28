using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navigation_Service
{
    internal class PositionArrivedEventArgs  : EventArgs
    {
        public  GNSSPosition PositionData { get; private set; }

        public PositionArrivedEventArgs(GNSSPosition position)
        {
            PositionData = position;
        }
    }
}
