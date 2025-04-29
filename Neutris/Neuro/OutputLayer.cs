namespace Neutris.Neuro
{
    internal class OutputLayer(Neuron[] neurons) : Layer(neurons)
    {
        public double GetResult()
        {
            var max = double.MinValue;
            var maxIndex = -1;
            for (int i = 0; i < neurons.Length; i++)
            {
                var res = neurons[i].GetResult();

                if (res > max)
                {
                    max = res;
                    maxIndex = i;
                }
            }
            return maxIndex;
        }
    }
}
