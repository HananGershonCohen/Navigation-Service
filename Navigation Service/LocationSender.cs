using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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

        public void Start()
        {
            Task.Run(async () =>
            {
                _logger.Information("[LocationSender] Starting to send locations to simulator.");
                var random = new Random();

                while (true)
                {
                    try
                    {
                        string location = $"{_state.Latitude:F6},{_state.Longitude:F6},{_state.Altitude:F6}";
                        byte[] data = Encoding.UTF8.GetBytes(location);

                        // Send location to simulator
                        await _udpClient.SendAsync(data, data.Length, _simulatorEndpoint);

                        _logger.Information("[LocationSender] Sent location: {Location}", location);

                        await Task.Delay(1000); // Wait 1 second
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "[LocationSender] Failed to send location.");
                    }
                }
            });
        }
    }
}