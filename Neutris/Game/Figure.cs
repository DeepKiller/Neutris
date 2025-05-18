using System.Xml.Linq;

namespace Neutris.Game
{
    internal class Figure(int[,] map)
    {
        public uint X { get; protected set; } = Field.WIDTH / 2 - 2;
        public uint Y { get; protected set; }

        public int[,] Map { get; private set; } = map;

        public Figure(int[,] map, uint x, uint y) : this(map)
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
            var newMap = new int[Map.GetLength(1), Map.GetLength(0)];
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

        public Rotation Rotation { get; private set; } = Rotation.deg0;

        private void SetRotation(bool forward = true)
        {
            if (forward)
            {
                if (Rotation != Rotation.deg270)
                {
                    Rotation = (Rotation)(((int)Rotation) << 1);
                }
                else
                {
                    Rotation = Rotation.deg0;
                }
            }
            else
            {
                if (Rotation != Rotation.deg0)
                {
                    Rotation = (Rotation)(((int)Rotation) >> 1);
                }
                else
                {
                    Rotation = Rotation.deg270;
                }
            }
        }

        public void Rotate(Field field)
        {
            InnerRotate(false, field);
            SetRotation();

            if (field.HasIntersection(this))
            {
                InnerRotate(true, field);
                SetRotation(false);
            }
        }
    }

    [Flags]
    enum Rotation
    {
        deg0 = 1,
        deg90 = 2,
        deg180 = 4,
        deg270 = 8
    }

    enum Direction
    {
        Left,
        Right
    }

    internal class OFigure : Figure
    {
        static readonly int[,] map = new int[2, 2]
        {
            { 1, 1 },
            { 1, 1 }
        };

        public OFigure() : base(map)
        {

        }
    }

    internal class LFigure : Figure
    {
        static readonly int[,] map = new int[2, 3]
            {
                {0,0,2},
                {2,2,2},
            };

        public LFigure() : base(map)
        {

        }
    }
    internal class JFigure : Figure
    {
        static readonly int[,] map = new int[2, 3]
            {
                {3,3,3},
                {0,0,3},
            };

        public JFigure() : base(map)
        {

        }
    }

    internal class TFigure : Figure
    {
        static readonly int[,] map = new int[2, 3]
            {
                {0,4,0},
                {4,4,4},
            };

        public TFigure() : base(map)
        {

        }
    }

    internal class IFigure : Figure
    {
        static readonly int[,] map = new int[1, 4]
            {
                {5,5,5,5},
            };

        public IFigure() : base(map)
        {

        }
    }

    internal class SFigure : Figure
    {
        static readonly int[,] map = new int[2, 3]
            {
                {0,6,6},
                { 6,6,0}
            };

        public SFigure() : base(map)
        {

        }
    }

    internal class ZFigure : Figure
    {
        static readonly int[,] map = new int[2, 3]
            {
                {7,7,0},
                { 0,7,7}
            };

        public ZFigure() : base(map)
        {

        }
    }
}
