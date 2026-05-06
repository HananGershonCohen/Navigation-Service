using System;
using System.Globalization;
using System.Text;
using Serilog;

namespace Navigation_Service
{
    internal class INSDevice : INavigationDevice
    {
        public event EventHandler<PositionArrivedEventArgs> onPositionArrived;

        private readonly ILogger _logger;
        private readonly ImuMeasurement _currentMeasurement = new ImuMeasurement();

        // Accept an IStreamSource (e.g. UdpSource) and subscribe to raw data.
        public INSDevice(IStreamSource source, ILogger logger)
        {
            _logger = logger.ForContext<INSDevice>();
            source.RawDataReceived += OnRawDataReceived;
            _logger.Information("[INSDevice] Subscribed to stream source.");
        }

        private void OnRawDataReceived(object sender, RawDataEventArgs e)
        {
            try
            {
                // Try parse as ASCII CSV: "timestamp,accelX,accelY,accelZ,gyroX,gyroY,gyroZ"
                string payload = Encoding.ASCII.GetString(e.Data).Trim();
                if (string.IsNullOrWhiteSpace(payload)) return;

                var parts = payload.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 7)
                {
                    // Example: parts[0] may be device timestamp; if not available we fallback to arrival time.
                    if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var ts))
                    {
                        ts = (e.Timestamp - DateTime.UnixEpoch).TotalSeconds;
                    }

                    _currentMeasurement.Timestamp = DateTime.UnixEpoch.AddSeconds(ts);
                    _currentMeasurement.AccelX = double.Parse(parts[1], CultureInfo.InvariantCulture);
                    _currentMeasurement.AccelY = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    _currentMeasurement.AccelZ = double.Parse(parts[3], CultureInfo.InvariantCulture);
                    _currentMeasurement.GyroX = double.Parse(parts[4], CultureInfo.InvariantCulture);
                    _currentMeasurement.GyroY = double.Parse(parts[5], CultureInfo.InvariantCulture);
                    _currentMeasurement.GyroZ = double.Parse(parts[6], CultureInfo.InvariantCulture);

                    _logger.Debug("[INSDevice] Parsed IMU: ts={Timestamp}, ax={AccelX}, ay={AccelY}, az={AccelZ}, gx={GyroX}, gy={GyroY}, gz={GyroZ}",
                        _currentMeasurement.Timestamp, _currentMeasurement.AccelX, _currentMeasurement.AccelY, _currentMeasurement.AccelZ,
                        _currentMeasurement.GyroX, _currentMeasurement.GyroY, _currentMeasurement.GyroZ);

                    // Raise the navigation event carrying the IMeasurement (ImuMeasurement implements IMeasurement)
                    onPositionArrived?.Invoke(this, new PositionArrivedEventArgs(_currentMeasurement));
                }
                else
                {
                    _logger.Warning("[INSDevice] Unexpected INS payload format: {Payload}", payload);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[INSDevice] Failed to parse INS data: {Message}", ex.Message);
            }
        }
    }
}