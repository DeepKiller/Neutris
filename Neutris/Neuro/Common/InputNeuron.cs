namespace Neutris.Neuro.Common
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
