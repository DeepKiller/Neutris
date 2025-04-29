namespace Neutris.Game
{
    using System.Windows;
    using Neutris.Graphic;

    internal class Field
    {
        internal const uint WIDTH = 10;
        internal const uint HEIGHT = 20;

        public bool[,] Map { get; private set; } = new bool[WIDTH, HEIGHT];
        private Figure? LastFigure;

        public void FixFigure(Figure figure)
        {
            for (int xF = 0; xF < figure.Map.GetLength(0); xF++)
            {
                for (int yF = 0; yF < figure.Map.GetLength(1); yF++)
                {
                    if (figure.Map[xF, yF])
                    {
                        Map[xF + figure.X, yF + figure.Y] = true;
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
                        Map[x, y] = false;
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

                    if (pos)
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

        private bool[,] GetMapWithFigure()
        {
            var clone = Map.Clone() as bool[,];
            if (LastFigure is Figure figure && clone is not null)
            {
                for (int xF = 0; xF < figure.Map.GetLength(0); xF++)
                {
                    for (int yF = 0; yF < figure.Map.GetLength(1); yF++)
                    {
                        if (figure.Map[xF, yF])
                        {
                            clone[xF + figure.X, yF + figure.Y] = true;
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
                    if (figure.Map[xF, yF])
                    {
                        if (Map[xF + figure.X, yF + figure.Y])
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool[,] RenderCurrentState()
        {
            return GetMapWithFigure();
        }
    }
}
