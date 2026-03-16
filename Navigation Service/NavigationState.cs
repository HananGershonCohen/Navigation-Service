namespace Navigation_Service
{
    public class NavigationState
    {
        public double Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public double Latitude { get; set; } = 0.0;
        public double Longitude { get; set; } = 0.0;
        public double Altitude { get; set; } = 0.0;
        public double Roll { get; set; } = 0.0;
        public double Pitch { get; set; } = 0.0;
        public double Yaw { get; set; } = 0.0;
        public double SpeedMs { get; set; } = 0.0;

        // Indicates if the navigation state is ready for use (e.g., after initialization)
        public bool IsReady { get; set; } = false;
    }
}