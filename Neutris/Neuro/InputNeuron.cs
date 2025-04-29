namespace Neutris.Neuro
{
    internal class InputNeuron() : Neuron
    {
        public override double GetResult()
        {
            return value;
        }

        private double value;

        public void SetValue(double value)
        {
            this.value = value;
        }
    }
}
