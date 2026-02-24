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
        private readonly ILogger _logger;

        public LocationSender(int simulatorPort, ILogger logger)
        {
            _udpClient = new UdpClient();
            _simulatorEndpoint = new IPEndPoint(IPAddress.Loopback, simulatorPort);
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
                        // Generate random latitude and longitude
                        double latitude = random.NextDouble() * 180.0 - 90.0; // -90 to 90
                        double longitude = random.NextDouble() * 360.0 - 180.0; // -180 to 180

                        string location = $"{latitude:F6},{longitude:F6}";
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