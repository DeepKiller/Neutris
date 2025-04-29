using System.Drawing;
using Neutris.Game;

namespace Neutris.Neuro
{
    internal class Network
    {
        private List<Layer> layers = [];
        private InputLayer inputLayer;
        private OutputLayer outputLayer;

        private ulong score = 0;

        const int FIGURE_MAX_WIDTH = 4;
        const int FIGURE_MAX_HEIGHT = 4;

        const uint INPUT_LAYER_SIZE = 2 + FIGURE_MAX_WIDTH * FIGURE_MAX_HEIGHT + Field.WIDTH * Field.HEIGHT; // Координаты фигуры + Максимальные габариты фигуры + Поле

        public double[] reses;

        private static Random random = new Random();

        public ulong Score
        {
            get
            {
                //var notUsed = reses.LongCount(x => x == 0);

                //if (notUsed == 0)
                //{
                return score;
                //}
                //else
                //{
                //    return score / (ulong)notUsed;
                //}
            }
            set
            {
                score = value;
            }
        }

        public void Clear()
        {
            score = 0;
            for (int i = 0; i < reses.Length; i++)
            {
                reses[i] = 0;
            }
        }

        private void SetBase(int layers, int size)
        {
            List<InputNeuron> input = [];
            for (int i = 0; i < INPUT_LAYER_SIZE; i++)
            {
                input.Add(new InputNeuron());
            }
            inputLayer = new InputLayer([.. input]);

            List<Neuron> output = [];
            for (int i = 0; i < 4; i++)
            {
                output.Add(new Neuron());
            }
            outputLayer = new OutputLayer([.. output]);

            reses = new double[output.Count];

            for (int i = 0; i < layers; i++)
            {
                List<Neuron> layer = [];

                for (int j = 0; j < size; j++)
                {
                    layer.Add(new Neuron());
                }

                this.layers.Add(new Layer([.. layer]));
            }
        }

        public Network(int layers, int size)
        {
            SetBase(layers, size);

            inputLayer!.Connect(this.layers.First());

            for (int i = 0; i < layers - 1; i++)
            {
                this.layers[i].Connect(this.layers[i + 1]);
            }

            this.layers.Last().Connect(outputLayer!);
        }

        public Network(Network parent1, Network parent2, int layers, int size)
        {
            SetBase(layers, size);

            var isMutant = random.NextDouble() > 0.95;

            inputLayer!.Connect(this.layers.First(), parent1.layers.First(), parent2.layers.First(), isMutant);

            for (int i = 0; i < layers - 1; i++)
            {
                this.layers[i].Connect(this.layers[i + 1], parent1.layers[i + 1], parent2.layers[i + 1], isMutant);
            }

            this.layers.Last().Connect(outputLayer!, parent1.outputLayer, parent2.outputLayer, isMutant);
        }

        public double Predict(Figure figure, Field field)
        {
            double[,] figureArray = new double[FIGURE_MAX_WIDTH, FIGURE_MAX_HEIGHT];
            double[,] fieldArray = new double[Field.WIDTH, Field.HEIGHT];

            for (int x = 0; x < figure.Map.GetLength(0); x++)
            {
                for (int y = 0; y < figure.Map.GetLength(1); y++)
                {
                    figureArray[x, y] = figure.Map[x, y] ? 1 : 0;
                }
            }

            for (int x = 0; x < field.Map.GetLength(0); x++)
            {
                for (int y = 0; y < field.Map.GetLength(1); y++)
                {
                    fieldArray[x, y] = field.Map[x, y] ? 1 : 0;
                }
            }

            inputLayer.SetValues([figure.X, figure.Y, .. figureArray, .. fieldArray]);

            var res = outputLayer.GetResult();
            reses[(int)res]++;
            return res;
        }
    }
}
