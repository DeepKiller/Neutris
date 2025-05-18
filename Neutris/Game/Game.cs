using Neutris.Graphic;
using Neutris.Neuro.FNN;

namespace Neutris.Game
{
    internal class Game
    {
        Field Field = new();
        FigureQueue FigureQueue = new(1);
        Figure? Current;

        private bool Control(DisplayWindow<Network> window)
        {
            if (Current is not null)
            {
                var input = Console.ReadKey(true);

                if (input.KeyChar == 'a')
                {
                    Current.Move(Direction.Left, Field);
                    Field.DrawFigure(Current);
                }

                if (input.KeyChar == 'd')
                {
                    Current.Move(Direction.Right, Field);
                    Field.DrawFigure(Current);
                }

                if (input.KeyChar == 's')
                {
                    if (Current.MoveDown(Field))
                    {
                        Field.FixFigure(Current);
                        Current = FigureQueue.GetFigure();
                        if (Field.DrawFigure(Current))
                        {
                            return true;
                        }
                    }
                }

                if (input.KeyChar == 'w')
                {
                    Current.Rotate(Field);
                    Field.DrawFigure(Current);
                }
            }

            return false;
        }

        private static void SetPoints(ref ulong points, int lines)
        {
            points += lines switch
            {
                1 => 40,
                2 => 100,
                3 => 300,
                4 => 1200,
                _ => 0
            };
        }

        public void Play()
        {
            Current = FigureQueue.GetFigure();
            Field.DrawFigure(Current);
            using var window = new DisplayWindow<Network>(500, 500, "game", new Neuro.GameNeuro<Network>(1));
            ulong points = 0;
            ulong ticks = 0;
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    if (Control(window))
                    {
                        break;
                    }
                }
                else
                {
                    Task.Delay(500).Wait();
                    if (Current.MoveDown(Field))
                    {
                        Field.FixFigure(Current);
                        Current = FigureQueue.GetFigure();
                        if (Field.DrawFigure(Current))
                        {
                            break;
                        }
                        SetPoints(ref points, Field.ClearFullLines());
                    }
                    else
                    {
                        Field.DrawFigure(Current);
                    }
                }
                ticks++;
            }
            Field.FixFigure(Current);
            Console.WriteLine($"You score is: {points} points! survived: {ticks} ticks");
        }
    }
}
