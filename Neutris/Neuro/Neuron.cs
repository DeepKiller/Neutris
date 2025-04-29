using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neutris.Neuro
{
    internal class Neuron
    {
        public List<Synaps> Inputs = [];

        private static double ActivationFunction(double value)
        {
            var pow = Math.Pow(Math.E, 2 * value);

            return (pow - 1) / (pow + 1);
        }

        public void AddSynaps(Synaps synaps)
        {
            Inputs.Add(synaps);
        }

        public virtual double GetResult()
        {
            var value = 0d;
            foreach (var input in Inputs)
            {
                value += input.GetValue();
            }

            return ActivationFunction(value);
        }
    }
}
