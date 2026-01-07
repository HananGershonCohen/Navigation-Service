using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NmeaParser.Messages;
using Serilog;

namespace Navigation_Service
{
    public class UdpNmeaSource : INmeaSource
    {
        private UdpClient _udpClient;
        private int _port;
        private readonly ILogger _logger;

        public event EventHandler<NmeaMessage> MessageReceived;

        public UdpNmeaSource(int port,ILogger logger)
        {
            _logger = logger.ForContext<UdpNmeaSource>();
            _port = port;
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, _port));
        }

        public async Task Start()
        {
            _logger.Information($"[UDP Source] Listening on port {_port}");
            // loop listening for incoming datagram
            while (true)
            {
                try
                {
                    var result = await _udpClient.ReceiveAsync();
                    byte[] rawBytes = result.Buffer;
                    _logger.Debug($"[UDP Source] Received {rawBytes.Length} bytes");

                    string sentence = Encoding.ASCII.GetString(rawBytes).Trim();

                    try
                    {
                        // parse NMEA sentence to NmeaMessage object.
                        // If the sentence is not recognized, an exception is thrown.
                        _logger.Debug($"[UDP Source] Parsing NMEA sentence: {sentence}");
                        var msg = NmeaMessage.Parse(sentence);
                        MessageReceived?.Invoke(this, msg);
                    }
                    catch (Exception parseEx)
                    {
                        _logger.Warning($"[UDP Source] Failed to parse NMEA sentence: {parseEx.Message}");
                    }
                }
                catch (ObjectDisposedException)
                {
                    _logger.Information("[UDP Source] UDP client has been closed. Stopping reception.");
                    break; 
                }
                catch (Exception ex)
                {
                    _logger.Error($"[UDP Source] Receive Error: {ex.Message}");
                }
            }
        }

        public Task Stop()
        {
            _udpClient?.Close();
            return Task.CompletedTask;
        }
    }
}