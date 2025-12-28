using System;
using NmeaParser.Messages;

namespace Navigation_Service
{
    public class VtgMapper : INmeaMapper
    {
        public void Map(NmeaMessage message, GNSSPosition target)
        {
            if (message is Vtg vtg)
            {
                if (!double.IsNaN(vtg.SpeedKph))
                {
                    target.SpeedMs = vtg.SpeedKph / 3.6;
                }
                if (!double.IsNaN(vtg.CourseTrue))
                {
                    target.CourseRad = vtg.CourseTrue * (Math.PI / 180.0);
                }
            }
        }
    }
}