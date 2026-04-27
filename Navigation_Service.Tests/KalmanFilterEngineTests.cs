using MathNet.Numerics.LinearAlgebra;
using Xunit;

namespace Navigation_Service.Tests
{
    public class KalmanFilterEngineTests
    {
        [Fact]
        public void CarInFogScenario_ShouldBlendPredictionAndMeasurement()
        {
            // Initialize state vector (9D, all zeros)
            var x = Vector<double>.Build.Dense(9);

            // Initialize covariance matrix (Identity * 0.1)
            var P = Matrix<double>.Build.DenseIdentity(9) * 0.1;

            // Process noise covariance (Identity * 0.01)
            var Q = Matrix<double>.Build.DenseIdentity(9) * 0.01;

            // Measurement noise covariance (Identity * 0.1)
            var R = Matrix<double>.Build.DenseIdentity(3) * 0.1;

            // Measurement matrix (Extract position from state)
            var H = Matrix<double>.Build.Dense(3, 9);
            H[0, 0] = 1; // Position X
            H[1, 1] = 1; // Position Y
            H[2, 2] = 1; // Position Z

            // Create Kalman Filter Engine
            var kalmanFilter = new KalmanFilterEngine(x, P, Q, R, H);

            // Predict step: Simulate driver moving 1 meter in X direction
            var F = Matrix<double>.Build.DenseIdentity(9);
            F[0, 3] = 1; // Position_X += Velocity_X * dt
            x[3] = 1.0; // Velocity_X = 1.0
            kalmanFilter.Predict(F);

            // Assert: Position X should now be exactly 1.0
            Assert.Equal(1.0, kalmanFilter.GetState()[0], 5);

            // Update step: GPS reports position as 1.2 meters
            var z = Vector<double>.Build.DenseOfArray(new double[] { 1.2, 0, 0 });
            kalmanFilter.Update(z);

            // Assert: Position X should be between 1.0 and 1.2
            var filteredPositionX = kalmanFilter.GetState()[0];

            Console.WriteLine($"------------------------------------");
            Console.WriteLine($"Prediction (Driver): 1.000");
            Console.WriteLine($"Measurement (GPS): 1.200");
            Console.WriteLine($"Kalman Result: {filteredPositionX:F4}");
            Console.WriteLine($"------------------------------------");

            Assert.InRange(filteredPositionX, 1.0001, 1.1999);
        }
    }
}