using MathNet.Numerics.LinearAlgebra;
using Xunit;
using System;
using System.Collections.Generic;

namespace Navigation_Service.Tests
{
    public class TrajectorySimulationTest
    {
        [Fact]
        public void Simulate50StepJourney_ShouldSmoothNoisyGPSData()
        {
            // Initialize state vector (9D, starting at 0 with velocity 10 m/s)
            var x = Vector<double>.Build.Dense(9);
            x[3] = 10.0; // Velocity_X = 10 m/s

            // Initialize covariance matrix (Identity * 0.1)
            var P = Matrix<double>.Build.DenseIdentity(9) * 0.1;

            // Process noise covariance (Identity * 0.01)
            var Q = Matrix<double>.Build.DenseIdentity(9) * 0.01;

            // Measurement noise covariance (Identity * 5.0)
            var R = Matrix<double>.Build.DenseIdentity(3) * 5.0;

            // Measurement matrix (Extract position from state)
            var H = Matrix<double>.Build.Dense(3, 9);
            H[0, 0] = 1; // Position X
            H[1, 1] = 1; // Position Y
            H[2, 2] = 1; // Position Z

            // Create Kalman Filter Engine
            var kalmanFilter = new KalmanFilterEngine(x, P, Q, R, H);

            // Simulation parameters
            const int steps = 50;
            const double dt = 1.0;
            const double velocity = 10.0;
            var random = new Random();

            // Data storage
            var truePositions = new List<double>();
            var noisyGPSPositions = new List<double>();
            var filteredPositions = new List<double>();

            // Simulation loop
            for (int i = 0; i < steps; i++)
            {
                // True position
                double truePosition = i * velocity * dt;
                truePositions.Add(truePosition);

                // Noisy GPS measurement
                double noisyGPS = truePosition + (random.NextDouble() * 10.0 - 5.0); // Noise between -5 and +5
                noisyGPSPositions.Add(noisyGPS);

                // Predict step
                var F = Matrix<double>.Build.DenseIdentity(9);
                F[0, 3] = dt; // Position_X += Velocity_X * dt
                kalmanFilter.Predict(F);

                // Update step
                var z = Vector<double>.Build.DenseOfArray(new double[] { noisyGPS, 0, 0 });
                kalmanFilter.Update(z);

                // Store filtered position
                double filteredPosition = kalmanFilter.GetState()[0];
                filteredPositions.Add(filteredPosition);
            }

            // Output results
            Console.WriteLine("Step\tTrue Position\tNoisy GPS\tFiltered Position");
            for (int i = 0; i < steps; i++)
            {
                Console.WriteLine($"{i}\t{truePositions[i]:F2}\t{noisyGPSPositions[i]:F2}\t{filteredPositions[i]:F2}");
            }

            // Calculate total errors
            double rawGPSError = 0.0;
            double filteredError = 0.0;
            for (int i = 0; i < steps; i++)
            {
                rawGPSError += Math.Abs(noisyGPSPositions[i] - truePositions[i]);
                filteredError += Math.Abs(filteredPositions[i] - truePositions[i]);
            }

            Console.WriteLine($"\nTotal Raw GPS Error: {rawGPSError:F2}");
            Console.WriteLine($"Total Filtered Error: {filteredError:F2}");

            // Assert that the filtered error is significantly lower than the raw GPS error
            Assert.True(filteredError < rawGPSError, "Kalman should at least be better than raw GPS");
        }
    }
}