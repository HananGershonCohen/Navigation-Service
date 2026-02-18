using System;
using NmeaParser.Messages;
using Serilog;
namespace Navigation_Service
{
    internal class GNSSDevice : INavigationDevice
    {
        public event EventHandler<PositionArrivedEventArgs> onPositionArrived; // raise

        private GNSSPosition _currentPosition = new GNSSPosition();
        private readonly Dictionary<Type, INmeaMapper> _mappers;
        private readonly ILogger _logger;
        public GNSSDevice(NmeaParser messege , ILogger logger)
        {
            _logger = logger;
            messege.sentceNMEArecived += OnNmeaMessageReceived; // regist . (now : from NmeaParser)
            _logger.Information("[GNSSDevice] Subscribed to NMEA parser.");
            _mappers = new Dictionary<Type, INmeaMapper>
            {
                { typeof(Gga), new GgaMapper() },
                { typeof(Vtg), new VtgMapper() },
                // { typeof(Rmc), new RmcMapper() },

            };
        }

        private void OnNmeaMessageReceived(object sender, NmeaMessage message)
        {
            Type msgType = message.GetType();

            // There is no mapping for this message type..
            if (_mappers.TryGetValue(msgType, out var processor))
            {
                // map this messege.
                processor.Map(message, _currentPosition);

                // Raise an event or log the updated position.
                onPositionArrived?.Invoke(this, new PositionArrivedEventArgs(_currentPosition));
                
                // for test OR logger.
               _logger.Debug($"[GNSSDevice] Pos Updated: Lat={_currentPosition.Latitude:F6}, Lon={_currentPosition.Longitude:F6} (Src: {msgType.Name})");
            }
            else
            {
                Console.WriteLine("There is no mapping for this message type.");
            }
        }
    }
}