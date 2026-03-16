namespace Navigation_Service
{
    public interface IInertialMeasurementSource
    {
        double AccelX { get; }
        double AccelY { get; }
        double AccelZ { get; }
        double GyroX { get; }
        double GyroY { get; }
        double GyroZ { get; }
    }
}
