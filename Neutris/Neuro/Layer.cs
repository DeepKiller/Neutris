using System.Diagnostics;

namespace Neutris.Neuro
{
    internal class Layer(Neuron[] neurons)
    {
        private Neuron[] Neurons { get => neurons; }
        private static Random Random = new();

        public void Connect(Layer layer)
        {
            foreach (var myNeuron in neurons)
            {
                foreach (var layerNeuron in layer.Neurons)
                {
                    layerNeuron.AddSynaps(new Synaps(myNeuron));
                }
            }
        }

        private static double Mutate(double value, bool isMutant)
        {
            var rand = Random.NextDouble();

            if (isMutant && rand >= 0 && rand <= 0.05)
            {
                rand = Random.NextDouble() - 0.5;
                return (rand * value) + value;
            }
            else
            {
                return value;
            }
        }

        private static double PickParentWeight(int indexMy, int indexLayer, Layer parent1, Layer parent2)
        {
            var rand = Random.NextDouble();

            if (rand >= 0.5)
            {
                return parent1.Neurons[indexMy].Inputs[indexLayer].weight;
            }
            else
            {
                return parent2.Neurons[indexMy].Inputs[indexLayer].weight;
            }
        }

        public void Connect(Layer layer, Layer parent1, Layer parent2, bool isMutant)
        {
            for (int i1 = 0; i1 < neurons.Length; i1++)
            {
                Neuron? myNeuron = neurons[i1];
                for (int i = 0; i < layer.Neurons.Length; i++)
                {
                    Neuron? layerNeuron = layer.Neurons[i];
                    layerNeuron.AddSynaps(new Synaps(myNeuron, Mutate(PickParentWeight(i, layerNeuron.Inputs.Count, parent1, parent2), isMutant)));
                }
            }
        }
    }
}
