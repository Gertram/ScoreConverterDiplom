namespace ScoreConverter.ScorePresets
{
    public class Range : IRange
    {
        public Range(decimal start, decimal end)
        {
            SetStart(start);
            SetEnd(end);
        }

        private decimal start;

        public decimal GetStart() => start;
        public decimal Start => start;
        public decimal End => end;
        private void SetStart(decimal value) => start = value;

        private decimal end;

        public decimal GetEnd() => end;

        private void SetEnd(decimal value) => end = value;

        public bool InRange(decimal value) => value >= GetStart() && value <= GetEnd();
    }
}
