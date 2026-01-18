using System;
using System.Text;
using NmeaParser.Messages;
using Serilog;

namespace Navigation_Service
{
    public class NmeaParser 
    {
        private readonly ILogger _logger;

        public event EventHandler<NmeaMessage> sentceNMEArecived;

        public NmeaParser(IStreamSource streamSource, ILogger logger)
        {
            _logger = logger.ForContext<NmeaParser>();
            streamSource.RawDataReceived += OnRawDataReceived; // regist . 
        }

        private void OnRawDataReceived(object sender, RawDataEventArgs e)
        {
            try
            {
                string sentence = Encoding.ASCII.GetString(e.Data).Trim();

                if (string.IsNullOrWhiteSpace(sentence)) return;

                _logger.Debug("[NmeaParser] Parsing sentence: {Sentence}", sentence);
                var msg = NmeaMessage.Parse(sentence);

                sentceNMEArecived?.Invoke(this, msg); // raise .

            }
            catch (Exception ex)
            {
                _logger.Warning("[NmeaParser] Failed to parse data: {Error}", ex.Message);
            }
        }
    }
}