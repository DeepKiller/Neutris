namespace Neutris.Neuro.Common
{
    internal class OutputLayer(Neuron[] neurons) : Layer(neurons)
    {
        public bool[] GetResult()
        {
            var res = new bool[neurons.Length];
            for (int i = 0; i < neurons.Length; i++)
            {
                res[i] = Neurons[i].IsActive();
            }
            return res;
        }
    }
}
