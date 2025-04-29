namespace Neutris.Game
{
    using System.Windows;
    using Neutris.Graphic;

    internal class Field
    {
        internal const uint WIDTH = 10;
        internal const uint HEIGHT = 20;

        public int[,] Map { get; private set; } = new int[WIDTH, HEIGHT];
        private Figure? LastFigure;

        public void FixFigure(Figure figure)
        {
            for (int xF = 0; xF < figure.Map.GetLength(0); xF++)
            {
                for (int yF = 0; yF < figure.Map.GetLength(1); yF++)
                {
                    if (figure.Map[xF, yF] > 0)
                    {
                        Map[xF + figure.X, yF + figure.Y] = figure.Map[xF, yF];
                    }
                }
            }
        }

        private void DeleteLine(int line)
        {
            for (int y = line; y >= 0; y--)
            {
                if (y == 0)
                {
                    for (int x = 0; x < WIDTH; x++)
                    {
                        Map[x, y] = 0;
                    }
                }
                else
                {
                    for (int x = 0; x < WIDTH; x++)
                    {
                        Map[x, y] = Map[x, y - 1];
                    }
                }
            }
        }

        public int ClearFullLines()
        {
            var counter = 0;

            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    var pos = Map[x, y];

                    if (pos > 0)
                    {
                        if (x == WIDTH - 1)
                        {
                            counter++;
                            DeleteLine(y);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return counter;
        }

        public bool DrawFigure(Figure figure)
        {
            if (HasIntersection(figure))
            {
                return true;
            }

            LastFigure = new Figure(figure.Map, figure.X, figure.Y);
            return false;
        }

        private int[,] GetMapWithFigure()
        {
            var clone = Map.Clone() as int[,];
            if (LastFigure is Figure figure && clone is not null)
            {
                for (int xF = 0; xF < figure.Map.GetLength(0); xF++)
                {
                    for (int yF = 0; yF < figure.Map.GetLength(1); yF++)
                    {
                        if (figure.Map[xF, yF] > 0)
                        {
                            clone[xF + figure.X, yF + figure.Y] = figure.Map[xF, yF];
                        }
                    }
                }

                return clone;
            }

            return Map;
        }

        public bool HasIntersection(Figure figure)
        {
            for (int xF = 0; xF < figure.Map.GetLength(0); xF++)
            {
                for (int yF = 0; yF < figure.Map.GetLength(1); yF++)
                {
                    if (figure.Map[xF, yF] > 0)
                    {
                        if (Map[xF + figure.X, yF + figure.Y] > 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public int[,] RenderCurrentState()
        {
            return GetMapWithFigure();
        }
    }
}
