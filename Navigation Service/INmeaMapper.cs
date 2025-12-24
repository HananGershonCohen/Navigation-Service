using NmeaParser;
using NmeaParser.Messages;

namespace Navigation_Service
{
    internal interface INmeaMapper
    {
        // הממשק מקבל את מחלקת הבסיס NmeaMessage
        void Map(NmeaMessage message, GPSPosition target);
    }
}