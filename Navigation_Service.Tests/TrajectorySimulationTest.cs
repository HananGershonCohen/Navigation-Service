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
            Console.WriteLine("--- GPS Smoothing Test ---");
            Console.WriteLine("Step\tTrue Pos\tNoisy GPS\tFiltered Pos");
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

            Assert.True(filteredError < rawGPSError, "Kalman should at least be better than raw GPS");
        }

        [Fact]
        public void SensorFusion_DriftingINS_vs_NoisyGPS_ShouldOutperformBoth()
        {
            // Initialize Kalman Filter
            var x = Vector<double>.Build.Dense(9);
            var P = Matrix<double>.Build.DenseIdentity(9) * 0.1;
            var Q = Matrix<double>.Build.DenseIdentity(9) * 0.1;
            var R = Matrix<double>.Build.DenseIdentity(3) * 5.0;
            var H = Matrix<double>.Build.Dense(3, 9);
            H[0, 0] = 1; H[1, 1] = 1; H[2, 2] = 1;

            var kalmanFilter = new KalmanFilterEngine(x, P, Q, R, H);

            // Simulation parameters
            const int steps = 50;
            const double dt = 1.0;
            const double trueVelocity = 10.0;
            const double insVelocity = 10.5; // Constant drift of 0.5 m/s
            var random = new Random();

            // Data storage for detailed table
            var trueLog = new List<double>();
            var insLog = new List<double>();
            var gpsLog = new List<double>();
            var kalmanLog = new List<double>();

            double currentTruePos = 0.0;
            double currentInsOnlyPos = 0.0;

            // Simulation loop
            for (int i = 0; i < steps; i++)
            {
                currentTruePos += trueVelocity * dt;
                currentInsOnlyPos += insVelocity * dt;
                double gpsPos = currentTruePos + (random.NextDouble() * 10.0 - 5.0);

                // Predict step: Using drifting INS velocity
                var F = Matrix<double>.Build.DenseIdentity(9);
                F[0, 3] = dt;
                x[3] = insVelocity; // Simulating velocity input from INS
                kalmanFilter.Predict(F);

                // Update step: Using noisy GPS position
                var z = Vector<double>.Build.DenseOfArray(new double[] { gpsPos, 0, 0 });
                kalmanFilter.Update(z);

                // Log results
                trueLog.Add(currentTruePos);
                insLog.Add(currentInsOnlyPos);
                gpsLog.Add(gpsPos);
                kalmanLog.Add(kalmanFilter.GetState()[0]);
            }

            // Output Detailed Table
            Console.WriteLine("--- SENSOR FUSION: INS DRIFT VS NOISY GPS ---");
            Console.WriteLine("Step | True Pos | INS (Drift) | GPS (Noisy) | KALMAN (Fused)");
            Console.WriteLine("------------------------------------------------------------");
            for (int i = 0; i < steps; i++)
            {
                Console.WriteLine($"{i:D2}   | {trueLog[i]:F2}    | {insLog[i]:F2}       | {gpsLog[i]:F2}      | {kalmanLog[i]:F2}");
            }

            // Calculate errors
            double totalInsError = 0;
            double totalGpsError = 0;
            double totalKalmanError = 0;

            for (int i = 0; i < steps; i++)
            {
                totalInsError += Math.Abs(insLog[i] - trueLog[i]);
                totalGpsError += Math.Abs(gpsLog[i] - trueLog[i]);
                totalKalmanError += Math.Abs(kalmanLog[i] - trueLog[i]);
            }

            Console.WriteLine("\n--- SUMMARY ---");
            Console.WriteLine($"Total INS Error (Drift): {totalInsError:F2}");
            Console.WriteLine($"Total GPS Error (Noise): {totalGpsError:F2}");
            Console.WriteLine($"Total Kalman Error (Fused): {totalKalmanError:F2}");

            // Assertions
            Assert.True(totalKalmanError < totalGpsError, "Kalman Filter must be more accurate than noisy GPS.");
            Assert.True(totalKalmanError < totalInsError, "Kalman Filter must prevent the INS drift.");
        }
    }
}