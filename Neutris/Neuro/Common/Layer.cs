using System.Diagnostics;
using Neutris.Neuro.FNN;

namespace Neutris.Neuro.Common
{
    internal class Layer(Neuron[] neurons)
    {
        public Neuron[] Neurons { get => neurons; }
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

        private static double[] Mutate(double[] value, bool[] isMutant)
        {
            for (int i = 0; i < value.Length; i++)
            {
                var rand = Random.NextDouble();
                var mutate = isMutant[i];

                if (mutate && rand >= 0 && rand <= 0.05)
                {
                    rand = Random.NextDouble() * 4 - 2;
                    value[i] = value[i] + rand;
                }
            }

            return value;
        }

        private static double[] PickParentWeight(int indexMy, int indexLayer, Layer parent1, Layer parent2)
        {
            var rand = Random.NextDouble();

            if (rand >= 0.5)
            {
                return [parent1.Neurons[indexMy].Inputs[indexLayer].Weight, parent2.Neurons[indexMy].Inputs[indexLayer].Weight];
            }
            else
            {
                return [parent2.Neurons[indexMy].Inputs[indexLayer].Weight, parent1.Neurons[indexMy].Inputs[indexLayer].Weight];
            }
        }

        public static void Connect(Layer child1In, Layer child1Out, Layer child2In, Layer child2Out, Layer parent1, Layer parent2, bool[] isMutant)
        {
            for (int inNeurons = 0; inNeurons < child1In.Neurons.Length; inNeurons++)
            {
                Neuron? child1InNeuron = child1In.Neurons[inNeurons];
                Neuron? child2InNeuron = child2In.Neurons[inNeurons];

                for (int outNeurons = 0; outNeurons < child1Out.Neurons.Length; outNeurons++)
                {
                    Neuron? child1OutNeuron = child1Out.Neurons[outNeurons];
                    Neuron? child2OutNeuron = child2Out.Neurons[outNeurons];

                    var parentsWeights = Mutate(PickParentWeight(outNeurons, child1OutNeuron.Inputs.Count, parent1, parent2), isMutant);

                    child1OutNeuron.AddSynaps(new Synaps(child1InNeuron, parentsWeights[0]));
                    child2OutNeuron.AddSynaps(new Synaps(child2InNeuron, parentsWeights[1]));
                }
            }
        }

        public void ClearCache()
        {
            foreach (var neuron in Neurons)
            {
                neuron.ClearCache();
            }
        }
    }
}
