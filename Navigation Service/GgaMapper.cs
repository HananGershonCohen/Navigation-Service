using NmeaParser.Messages;

namespace Navigation_Service
{
    public class GgaMapper : INmeaMapper
    {
        public void Map(NmeaMessage message, GNSSPosition target)
        {
            // safe cast to Gga from NmeaMessage.
            if (message is Gga gga)  
            {
                target.Latitude = gga.Latitude;
                target.Longitude = gga.Longitude;
                target.Altitude = gga.Altitude;
                target.IsValid = gga.Quality != Gga.FixQuality.Invalid;
                target.PosErrorH = gga.Hdop * 5.0;
                target.Timestamp = DateTime.UtcNow.Date.AddSeconds(gga.FixTime.TotalSeconds);
            }
        }
    }
}