using System.Drawing;
using Neutris.Game;
using Neutris.Neuro.Common;

namespace Neutris.Neuro.FNN
{
    internal class Network : INetwork
    {
        public List<Layer> Layers { get; private set; } = [];
        public InputLayer InputLayer { get; private set; }
        public OutputLayer OutputLayer { get; private set; }

        private ulong score = 0;

        const int FIGURE_MAX_WIDTH = 4;
        const int FIGURE_MAX_HEIGHT = 4;

        /// <summary>
        /// Тип фигуры (7), поворот фигуры (4), координаты фигуры (2), высота столбцов (10), перепад высот (1), количество пропущенных ячеек(1), средняя высота(1)
        /// </summary>
        const uint INPUT_LAYER_SIZE = 26;

        private static Random random = new();

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
        }

        private void SetBase(int layers, int size)
        {
            List<InputNeuron> input = [];
            for (int i = 0; i < INPUT_LAYER_SIZE; i++)
            {
                input.Add(new InputNeuron());
            }
            InputLayer = new InputLayer([.. input]);

            List<Neuron> output = [];
            for (int i = 0; i < 3; i++)
            {
                output.Add(new Neuron());
            }
            OutputLayer = new OutputLayer([.. output]);

            for (int i = 0; i < layers; i++)
            {
                List<Neuron> layer = [];

                for (int j = 0; j < size; j++)
                {
                    layer.Add(new Neuron());
                }

                this.Layers.Add(new Layer([.. layer]));
            }
        }

        public Network(int layers, int size)
        {
            SetBase(layers, size);

            InputLayer!.Connect(this.Layers.First());

            for (int i = 0; i < layers - 1; i++)
            {
                this.Layers[i].Connect(this.Layers[i + 1]);
            }

            this.Layers.Last().Connect(OutputLayer!);
        }

        public Network()
        {

        }

        public static Network[] CrossOver(Network parent1, Network parent2, int layers, int size)
        {
            var child1 = new Network();
            child1.SetBase(layers, size);
            var child2 = new Network();
            child2.SetBase(layers, size);

            var isMutant1 = random.NextDouble() > 0.97;
            var isMutant2 = random.NextDouble() > 0.97;

            Layer.Connect(child1.InputLayer, child1.Layers.First(), child2.InputLayer, child2.Layers.First(), parent1.Layers.First(), parent2.Layers.First(), [isMutant1, isMutant2]);

            for (int i = 0; i < layers - 1; i++)
            {
                Layer.Connect(child1.Layers[i], child1.Layers[i + 1], child2.Layers[i], child2.Layers[i + 1], parent1.Layers[i + 1], parent2.Layers[i + 1], [isMutant1, isMutant2]);
            }

            Layer.Connect(child1.Layers.Last(), child1.OutputLayer, child2.Layers.Last(), child2.OutputLayer, parent1.OutputLayer, parent2.OutputLayer, [isMutant1, isMutant2]);

            return [child1, child2];
        }

        private double[] GetFigureType(Figure figure)
        {
            return [figure is OFigure ? 1 : 0,
                    figure is LFigure ? 1 : 0,
                    figure is JFigure ? 1 : 0,
                    figure is TFigure ? 1 : 0,
                    figure is IFigure ? 1 : 0,
                    figure is SFigure ? 1 : 0,
                    figure is ZFigure ? 1 : 0];
        }

        private double[] GetFigureRotation(Figure figure)
        {
            return [figure.Rotation == Rotation.deg0 ? 1 : 0,
                    figure.Rotation == Rotation.deg90 ? 1 : 0,
                    figure.Rotation == Rotation.deg180 ? 1 : 0,
                    figure.Rotation == Rotation.deg270 ? 1 : 0];
        }

        private double[] GetFieldSizes(Field field)
        {
            double[] cols = new double[Field.WIDTH];

            for (int i = 0; i < Field.WIDTH; i++)
            {
                cols[i] = Field.HEIGHT;
            }

            for (int x = 0; x < Field.WIDTH; x++)
            {
                for (int y = 0; y < Field.HEIGHT; y++)
                {
                    if (field.Map[x, y] > 0)
                    {
                        cols[x] = y;
                        break;
                    }
                }
            }

            return cols;
        }

        private double GetHeightDifference(double[] sizes)
        {
            var res = 0d;
            for (int i = 0; i < sizes.Length - 1; i++)
            {
                res += Math.Abs(sizes[i] - sizes[i + 1]);
            }

            return res;
        }

        private double CountEmpties(double[] sizes, Field field)
        {
            var counter = 0;
            for (int x = 0; x < Field.WIDTH; x++)
            {
                for (int y = (int)sizes[x]; y < Field.HEIGHT; y++)
                {
                    if (field.Map[x, y] == 0)
                    {
                        counter++;
                    }
                }
            }

            return counter;
        }

        public bool[] Predict(Figure figure, Field field)
        {
            Layer[] layers = [InputLayer, .. Layers, OutputLayer];
            foreach (Layer layer in layers)
            {
                layer.ClearCache();
            }

            var sizes = GetFieldSizes(field);

            InputLayer.SetValues(
                [
                 .. GetFigureType(figure),
                 .. GetFigureRotation(figure),
                 figure.X,
                 figure.Y,
                 .. sizes,
                 GetHeightDifference(sizes),
                 CountEmpties(sizes,field),
                 sizes.Average()
                 ]);

            var res = OutputLayer.GetResult();
            return res;
        }
    }
}
