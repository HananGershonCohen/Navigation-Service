namespace Navigation_Service
{
    public class NavigationState
    {
        public double Timestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Roll { get; set; }
        public double Pitch { get; set; }
        public double Yaw { get; set; }
        public double SpeedMs { get; set; }
    }
}