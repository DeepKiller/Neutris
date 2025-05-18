using System.Reflection;
using Neutris.Game;
using Neutris.Graphic;
using Neutris.Neuro.FNN;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Neutris.Neuro
{
    internal class GameNeuro<Tnet>(int seed) where Tnet : INetwork
    {
        Field Field = new();
        FigureQueue FigureQueue = new(seed);
        Figure? Current;
        public Tnet? Network { get; private set; }
        public ulong Points { get; private set; } = 0;
        public ulong Ticks { get; private set; } = 0;

        public int[,] Map { get => Field.RenderCurrentState(); }

        public void Clear(Tnet newGeneration)
        {
            Field = new();
            Current = null;
            FigureQueue = new(seed);
            Points = 0;
            Ticks = 0;
            Network = newGeneration;
        }

        public void Control()
        {
            if (Current is not null)
            {
                var input = Network!.Predict(Current, Field);

                if (input[0])
                {
                    Current.Move(Direction.Left, Field);
                    Field.DrawFigure(Current);
                }
                else if (input[1])
                {
                    Current.Move(Direction.Right, Field);
                    Field.DrawFigure(Current);
                }
                else if (input[2])
                {
                    Current.Rotate(Field);
                    Field.DrawFigure(Current);
                }
            }
        }

        private void SetPoints(int lines)
        {
            Points += lines switch
            {
                1 => 40,
                2 => 100,
                3 => 300,
                4 => 1200,
                _ => 0
            };
        }

        public void Play(Tnet network, bool visual)
        {
            Network = network;
            if (visual)
            {
                PlayVisual();
            }
            else
            {
                Play();
            }
        }

        private void Play()
        {
            Current = FigureQueue.GetFigure();
            Field.DrawFigure(Current);

            while (true)
            {
                if (Current.MoveDown(Field))
                {
                    Field.FixFigure(Current);
                    Current = FigureQueue.GetFigure();
                    if (Field.DrawFigure(Current))
                    {
                        break;
                    }
                    SetPoints(Field.ClearFullLines());
                }
                else
                {
                    Field.DrawFigure(Current);
                }
                Control();
                Ticks++;
            }
            //Console.WriteLine($"score is: {Points} points! survived: {Ticks} ticks");
        }

        internal void PrepareVisual()
        {
            Current = FigureQueue.GetFigure();
            Field.DrawFigure(Current);
        }

        internal bool UpdateVisual()
        {
            if (Current is not null)
            {
                Control();
                if (Current.MoveDown(Field))
                {
                    Field.FixFigure(Current);
                    Current = FigureQueue.GetFigure();
                    if (Field.DrawFigure(Current))
                    {
                        Field.FixFigure(Current);
                        return true;
                    }
                    SetPoints(Field.ClearFullLines());
                }
                else
                {
                    Field.DrawFigure(Current);
                }
                Ticks++;
            }

            return false;
        }

        private void PlayVisual()
        {
            using var window = new DisplayWindow<Tnet>(500, 500, "game", this);
            window.Run();
            Console.WriteLine($"score is: {Points} points! survived: {Ticks} ticks; score: {Network!.Score}");
        }
    }
}
