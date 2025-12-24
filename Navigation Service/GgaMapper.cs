using NmeaParser.Messages;

namespace Navigation_Service
{
    public class GgaMapper : INmeaMapper
    {
        public void Map(NmeaMessage message, GPSPosition target)
        {
            if (message is Gga gga)
            {
                target.Latitude = gga.Latitude;
                target.Longitude = gga.Longitude;
                target.Altitude = gga.Altitude;
                target.IsValid = gga.Quality != Gga.FixQuality.Invalid;
                target.PosErrorH = gga.Hdop * 5.0;
                target.Timestamp = gga.FixTime.TotalSeconds;
            }
        }
    }
}