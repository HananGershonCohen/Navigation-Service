using System.Collections.Generic;
using System.Text;
using NmeaParser.Messages;

namespace Navigation_Service
{
    internal class GPSDevice  : NavigationDevicesBase
    {
        private GPSPosition _currentPosition = new GPSPosition();
        private Dictionary<Type, INmeaMapper> _mappers;
        public GPSDevice()
        {
            _mappers = new Dictionary<Type, INmeaMapper>
            {
                { typeof(Gga), new GgaMapper() },
                // { typeof(Rmc), new RmcMapper() }, // future mappers can be added here
            };
        }

        public void On_RawDataReceived(object? sender, RawDataReceivedEventArgs e)    
        {
            byte[] rawData = e.RawData;
            try
            {
                string sentence = Encoding.ASCII.GetString(rawData).Trim();

                // using with NmeaParser .
                NmeaMessage msg = NmeaMessage.Parse(sentence);
                NmeaSentenceParsed(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Parse Error: {ex.Message}");
            }
        }

        private void NmeaSentenceParsed(NmeaMessage message)
        {
            Type messageType = message.GetType();

            if (_mappers.TryGetValue(messageType, out var mapper))
            {
                // start mapping the message to the current position object
                mapper.Map(message, _currentPosition);

                Console.WriteLine($"[GPS Device] Updated: {_currentPosition.Latitude}, {_currentPosition.Longitude} via {messageType.Name}");
            }
            else
            {
                Console.WriteLine($"[GPS Device] No mapper found for message type: {messageType.Name}");
                //....
            }
        }
    }
}