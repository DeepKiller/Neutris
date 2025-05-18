using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neutris.Neuro.Common
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

        private double? LastRes;

        public void ClearCache()
        {
            LastRes = null;
        }

        public bool IsActive()
        {
            return GetResult() > 0;
        }

        public virtual double GetResult()
        {
            LastRes ??= ActivationFunction(Inputs.Sum(x => x.GetValue()));

            return (double)LastRes;
        }
    }
}
