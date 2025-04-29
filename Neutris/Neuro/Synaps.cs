namespace Neutris.Neuro
{
    internal class Synaps
    {
        public double weight;
        private static readonly Random R = new();
        private Neuron From;

        public Synaps(Neuron from)
        {
            weight = R.NextDouble() - 0.5;
            From = from;
        }

        public Synaps(Neuron from, double weight)
        {
            this.weight = weight;
            From = from;
        }

        public double GetValue()
        {
            return weight * From.GetResult();
        }
    }
}
