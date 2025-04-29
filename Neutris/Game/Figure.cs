using System.Xml.Linq;

namespace Neutris.Game
{
    internal class Figure(bool[,] map)
    {
        public uint X { get; protected set; } = Field.WIDTH / 2 - 2;
        public uint Y { get; protected set; }

        public bool[,] Map { get; private set; } = map;

        public Figure(bool[,] map, uint x, uint y) : this(map)
        {
            X = x;
            Y = y;
        }

        public bool MoveDown(Field currentState)
        {
            if (Y < Field.HEIGHT - Map.GetLength(1))
            {
                Y++;

                if (currentState.HasIntersection(this))
                {
                    Y--;
                    return true;
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        public void Move(Direction direction, Field field)
        {
            if (direction == Direction.Left)
            {
                if (X > 0)
                {
                    X--;
                    if (field.HasIntersection(this))
                    {
                        X++;
                    }
                }
            }
            else
            {
                if (X < Field.WIDTH - Map.GetLength(0))
                {
                    X++;
                    if (field.HasIntersection(this))
                    {
                        X--;
                    }
                }
            }
        }

        private void InnerRotate(bool reverse, Field field)
        {
            var newMap = new bool[Map.GetLength(1), Map.GetLength(0)];
            var nx = 0;
            var ny = 0;

            if (reverse)
            {

                for (int x = Map.GetLength(0) - 1; x >= 0; x--)
                {
                    for (int y = 0; y < Map.GetLength(1); y++)
                    {
                        newMap[nx++, ny] = Map[x, y];
                    }
                    ny++;
                    nx = 0;
                }
            }
            else
            {
                for (int x = 0; x < Map.GetLength(0); x++)
                {
                    for (int y = Map.GetLength(1) - 1; y >= 0; y--)
                    {
                        newMap[nx++, ny] = Map[x, y];
                    }
                    ny++;
                    nx = 0;
                }
            }

            Map = newMap;

            if (Map.GetLength(1) + Y > Field.HEIGHT)
            {
                var diff = Y + (uint)Map.GetLength(1) - Field.HEIGHT;

                Y -= diff;
                if (field.HasIntersection(this))
                {
                    Y += diff;
                    InnerRotate(true, field);
                }
            }

            if (Map.GetLength(0) + X > Field.WIDTH)
            {
                var diff = X + (uint)Map.GetLength(0) - Field.WIDTH;

                X -= diff;
                if (field.HasIntersection(this))
                {
                    X += diff;
                    InnerRotate(true, field);
                }
            }
        }

        public void Rotate(Field field)
        {
            InnerRotate(false, field);

            if (field.HasIntersection(this))
            {
                InnerRotate(true, field);
            }
        }
    }

    enum Direction
    {
        Left,
        Right
    }

    internal class OFigure : Figure
    {
        static readonly bool[,] map = new bool[2, 2]
        {
            { true, true },
            { true, true }
        };

        public OFigure() : base(map)
        {

        }
    }

    internal class LFigure : Figure
    {
        static readonly bool[,] map = new bool[2, 3]
            {
                {false,false,true},
                {true,true,true},
            };

        public LFigure() : base(map)
        {

        }
    }
    internal class JFigure : Figure
    {
        static readonly bool[,] map = new bool[2, 3]
            {
                {true,true,true},
                {false,false,true},
            };

        public JFigure() : base(map)
        {

        }
    }

    internal class TFigure : Figure
    {
        static readonly bool[,] map = new bool[2, 3]
            {
                {false,true,false},
                {true,true,true},
            };

        public TFigure() : base(map)
        {

        }
    }

    internal class IFigure : Figure
    {
        static readonly bool[,] map = new bool[1, 4]
            {
                {true,true,true,true},
            };

        public IFigure() : base(map)
        {

        }
    }

    internal class SFigure : Figure
    {
        static readonly bool[,] map = new bool[2, 3]
            {
                {false,true,true},
                { true,true,false}
            };

        public SFigure() : base(map)
        {

        }
    }

    internal class ZFigure : Figure
    {
        static readonly bool[,] map = new bool[2, 3]
            {
                {true,true,false},
                { false,true,true}
            };

        public ZFigure() : base(map)
        {

        }
    }
}
