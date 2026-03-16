using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;

namespace Navigation_Service
{
    public class LocationSender
    {
        private readonly UdpClient _udpClient;
        private readonly IPEndPoint _simulatorEndpoint;
        private readonly NavigationState _state;
        private readonly ILogger _logger;

        public LocationSender(int simulatorPort, NavigationState state, ILogger logger)
        {
            _udpClient = new UdpClient();
            _simulatorEndpoint = new IPEndPoint(IPAddress.Loopback, simulatorPort);
            _state = state;
            _logger = logger.ForContext<LocationSender>();
        }

        public async Task SendCurrentStateAsync()
        {
            try
            {
                if (!_state.IsReady) return;

                string location = $"{_state.Timestamp:F6},{_state.Latitude:F6},{_state.Longitude:F6},{_state.Altitude:F6},{_state.Roll:F6},{_state.Pitch:F6},{_state.Yaw:F6},{_state.SpeedMs:F6}";
                byte[] data = Encoding.UTF8.GetBytes(location);

                await _udpClient.SendAsync(data, data.Length, _simulatorEndpoint);

                _logger.Debug("[LocationSender] Sent updated location: {Timestamp}", _state.Timestamp);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[LocationSender] Failed to send location.");
            }
        }
    }
}