using MathNet.Numerics.LinearAlgebra;

namespace Navigation_Service
{
    public class KalmanFilterEngine
    {
        private Vector<double> _x; // State Vector
        private Matrix<double> _P; // Covariance Matrix
        private readonly Matrix<double> _Q; // Process Noise Covariance
        private readonly Matrix<double> _R; // Measurement Noise Covariance
        private readonly Matrix<double> _H; // Measurement Matrix

        public KalmanFilterEngine(Vector<double> initialState, Matrix<double> initialCovariance,
                                   Matrix<double> processNoise, Matrix<double> measurementNoise,
                                   Matrix<double> measurementMatrix)
        {
            _x = initialState;
            _P = initialCovariance;
            _Q = processNoise;
            _R = measurementNoise;
            _H = measurementMatrix;
        }

        public void Predict(Matrix<double> F)
        {
            // Predict the next state: x = F * x
            _x = F * _x;

            // Predict the next covariance: P = F * P * F^T + Q
            _P = F * _P * F.Transpose() + _Q;
        }

        public void Update(Vector<double> z)
        {
            // Compute the innovation: y = z - (H * x)
            var y = z - (_H * _x);

            // Compute the innovation covariance: S = (H * P * H^T) + R
            var S = (_H * _P * _H.Transpose()) + _R;

            // Compute the Kalman Gain: K = P * H^T * S^-1
            var K = _P * _H.Transpose() * S.Inverse();

            // Update the state: x = x + (K * y)
            _x = _x + (K * y);

            // Update the covariance: P = (I - K * H) * P
            var I = Matrix<double>.Build.DenseIdentity(_P.RowCount);
            _P = (I - K * _H) * _P;
        }

        public Vector<double> GetState() => _x;
        public Matrix<double> GetCovariance() => _P;
    }
}