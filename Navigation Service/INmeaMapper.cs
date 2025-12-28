using NmeaParser.Messages;

namespace Navigation_Service
{
    internal interface INmeaMapper
    {
        // The interface receives the base class NmeaMessage
        void Map(NmeaMessage message, GNSSPosition target);
    }
}