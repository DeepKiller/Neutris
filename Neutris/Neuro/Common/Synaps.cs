namespace Neutris.Neuro.Common
{
    internal class Synaps
    {
        private double weight;
        private static readonly Random R = new();
        public Neuron From { get; private set; }

        const int clip = 2;
        public double Weight
        {
            get => weight; private set
            {
                if (value > clip)
                {
                    weight = clip;
                }
                else if (value < -clip)
                {
                    weight = -clip;
                }
                else
                {
                    weight = value;
                }
            }
        }

        public Synaps(Neuron from)
        {
            Weight = R.NextDouble() - 0.5;
            From = from;
        }

        public Synaps(Neuron from, double weight)
        {
            Weight = weight;
            From = from;
        }

        public double GetValue()
        {
            return Weight * From.GetResult();
        }
    }
}
