namespace MusicVis
{
    public class AdjustableMax
    {
        public float MinValue { get; private set; }
        public float CurrentMax { get; private set; }
        private float lastValue;
        private float value;
        public float Value
        {
            get
            {
                if (CurrentMax == 0)
                {
                    return 0;
                }
                float returningValue = value / CurrentMax;
                //if (returningValue < 0.1)
                //{
                //    return lastValue / currentMax;
                //}
                //else
                //{
                //    return returningValue;
                //}
                return returningValue;
            }
            set
            {
                this.lastValue = this.value;
                this.value = value;
                if (CurrentMax < value)
                {
                    CurrentMax = value;
                }
            }
        }

        public AdjustableMax()
        {
            CurrentMax = 0;
            value = 0;
        }

        public void Reset()
        {
            CurrentMax = 0;
            value = 0;
        }
    }
}
