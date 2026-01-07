using System;
using NmeaParser.Messages;
using Serilog;

namespace Navigation_Service
{
    internal class GNSSDevice : INavigationDevice
    {
        public event EventHandler<PositionArrivedEventArgs> onPositionArrived;
        private GNSSPosition _currentPosition = new GNSSPosition();
        private readonly Dictionary<Type, INmeaMapper> _mappers;
        private readonly ILogger _logger;
        public GNSSDevice(ILogger logger)
        {
            _logger = logger.ForContext<GNSSDevice>();

            _mappers = new Dictionary<Type, INmeaMapper>
            {
                { typeof(Gga), new GgaMapper() },
                { typeof(Vtg), new VtgMapper() },
                // { typeof(Rmc), new RmcMapper() },

            };
        }

        // function to connect from source
        public void ConnectSource(INmeaSource source)
        {
            _logger.Information("[GNSS Device] Connecting to NMEA source...");
            source.MessageReceived += OnNmeaMessageReceived;
            source.Start();
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
              
            }
            else
            {
                _logger.Debug($"[GNSS Device] No mapper for message type: {msgType.Name}");
            }
        }
    }
}