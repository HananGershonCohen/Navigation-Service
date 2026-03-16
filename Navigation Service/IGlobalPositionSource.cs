namespace Navigation_Service
{
    public interface IGlobalPositionSource
    {
        double Latitude { get; }
        double Longitude { get; }
        double Altitude { get; }
    }
}
