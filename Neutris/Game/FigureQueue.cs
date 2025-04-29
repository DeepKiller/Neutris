namespace Neutris.Game
{
    internal class FigureQueue
    {
        const int QUEUE_SIZE = 4;
        const int FIGURES_COUNT = 7;
        Random Random = new();
        Queue<Figure> Figures = new();

        public FigureQueue(int seed)
        {
            Random = new(seed);
            for (int i = 0; i < QUEUE_SIZE; i++)
            {
                Figures.Enqueue(SelectNext());
            }
        }

        public Figure GetFigure()
        {
            Figures.Enqueue(SelectNext());
            return Figures.Dequeue();
        }

        private Figure SelectNext()
        {
            var val = Random.Next(0, FIGURES_COUNT);

            return val switch
            {
                0 => new IFigure(),
                1 => new LFigure(),
                2 => new JFigure(),
                3 => new OFigure(),
                4 => new SFigure(),
                5 => new TFigure(),
                _ => new ZFigure(),
            };
        }
    }
}
