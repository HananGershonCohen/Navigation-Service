namespace Navigation_Service
{
    public interface IGlobalVelocitySource
    {
        double SpeedMs { get; }
        double CourseRad { get; } // Yaw
    }
}
