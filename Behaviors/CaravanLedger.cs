namespace RealisticEconomy.Behaviors
{
    public class CaravanLedger
    {
        private readonly float[] _window = new float[7];
        private int _index;
        private float _sum;

        public void AddDayProfit(float value)
        {
            _sum -= _window[_index];
            _window[_index] = value;
            _sum += value;
            _index = (_index + 1) % 7;
        }

        public float WeekSum => _sum;

        /// <summary>Zero the rolling sum after a caravan is disbanded.</summary>
        public void Reset() => _sum = 0f;            // ← NEW
    }
}


