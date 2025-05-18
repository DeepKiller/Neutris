using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neutris.Game;

namespace Neutris.Neuro
{
    internal interface INetwork
    {
        public bool[] Predict(Figure figure, Field field);

        public ulong Score { get; }
    }
}
