namespace Navigation_Service
{
    internal class PositionArrivedEventArgs  : EventArgs
    {
        public  IMeasurement _position { get; private set; }

        public PositionArrivedEventArgs(IMeasurement position)
        {
            _position = position;
        }
    }
}
