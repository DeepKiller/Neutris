using System.Diagnostics;

namespace Neutris.Neuro
{
    internal class Generation
    {
        Network[] networks;
        int size;
        int tries;
        int layers;
        int layerSize;
        ulong generationNumber = 0;
        static ulong bestScore = 0;

        int seed;

        static Random random = new();

        public Generation(int size, int tries, int layers, int layerSize, ulong generationNumber = 0)
        {
            networks = new Network[size];
            this.size = size;
            this.tries = tries;
            this.layers = layers;
            this.layerSize = layerSize;

            for (int i = 0; i < size; i++)
            {
                networks[i] = new Network(layers, layerSize);
            }
            this.generationNumber = generationNumber;

            seed = random.Next(int.MaxValue);
        }

        private void PlayAndCountBestPoints()
        {
            List<Task> tasks = [];

            foreach (var network in networks)
            {
                tasks.Add(Task.Run(() =>
                {
                    GameNeuro game = new(seed);
                    for (int @try = 0; @try < tries; @try++)
                    {
                        game.Play(network, false);
                        network.Score = game.Points > 0 ? game.Ticks * game.Points : game.Ticks;
                        game.Clear(network);
                    }
                }));
            }

            Task.WaitAll([.. tasks]);

            var best = networks.MaxBy(x => x.Score);
            if (best is not null)
            {
                if (best.Score > bestScore || generationNumber % 1 == 0)
                {
                    if (best.Score > bestScore)
                    {
                        bestScore = best.Score;
                    }
                    GameNeuro game = new(seed);
                    game.Play(best, true);
                }
            }
        }

        class NetworkPair
        {
            static Random random = new();

            public Network[] Pair { get; init; } = new Network[2];

            public NetworkPair(Network[] networks)
            {
                double summary = 0;

                foreach (var net in networks)
                {
                    summary += net.Score;
                }

                var count = networks.Length / 5;

                var sorted = networks.OrderByDescending(x => x.Score).Take((int)count);


                while (Pair[1] is null || Pair[0] is null)
                {
                    var chance1 = random.Next(100);
                    var chance2 = random.Next(100);
                    var n1Chance = 0d;
                    var n2Chance = 0d;

                    foreach (var net in sorted)
                    {
                        var percent = net.Score / summary * 100;
                        n1Chance += percent;
                        n2Chance += percent;

                        if (chance1 < n1Chance)
                        {
                            Pair[0] = net;
                            chance1 = int.MaxValue;
                            continue;
                        }

                        if (chance2 < n2Chance)
                        {
                            Pair[1] = net;
                            chance2 = int.MaxValue;
                        }
                    }
                }
            }

            public Network CrossOver(int layers, int size)
            {
                return new Network(Pair[0], Pair[1], layers, size);
            }
        }

        private Generation CrossingOver()
        {
            var nets = new Network[networks.Length];

            for (int i = 0; i < nets.Length; i++)
            {
                nets[i] = new NetworkPair(networks).CrossOver(layers, layerSize);
            }


            return new Generation(size, tries, layers, layerSize, ++generationNumber) { networks = nets };
        }

        public Generation MakeNewGeneration()
        {
            PlayAndCountBestPoints();
            Console.WriteLine($"generation: {generationNumber}; minimum points: {networks.Min(x => x.Score)} maximum points: {networks.Max(x => x.Score)}; overal maximum: {bestScore}");
            return CrossingOver();
        }
    }
}
