namespace Neutris.Neuro.Common
{
    internal class InputLayer(InputNeuron[] neurons) : Layer(neurons)
    {
        public void SetValues(double[] values)
        {
            if (neurons.Length == values.Length)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    neurons[i].SetValue(values[i]);
                }
            }
        }
    }
}
