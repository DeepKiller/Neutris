using System.Diagnostics;

namespace Neutris.Neuro.FNN
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
            Parallel.ForEach(networks, (network) =>
            {
                GameNeuro<Network> game = new(seed);
                for (int @try = 0; @try < tries; @try++)
                {
                    game.Play(network, false);
                    network.Score = (game.Ticks / 100) + (game.Points / 10);

                    if (network.Score == 0)
                    {
                        network.Score = 1;
                    }

                    game.Clear(network);
                }
            });

            var best = networks.MaxBy(x => x.Score);
            if (best is not null)
            {
                if (best.Score > bestScore || generationNumber % 1000 == 0)
                {
                    if (best.Score > bestScore)
                    {
                        bestScore = best.Score;
                    }
                    GameNeuro<Network> game = new(seed);
                    game.Play(best, true);
                }
            }
        }

        class NetworkPair
        {
            static Random random = new();

            public Network[] Pair { get; init; } = new Network[2];

            public NetworkPair(List<Network> networks)
            {
                double summary = 0;

                foreach (var net in networks)
                {
                    summary += net.Score;
                }

                var count = networks.Count;

                var sorted = networks.OrderByDescending(x => x.Score).Take(count);

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

                        if (chance1 < n1Chance && Pair[0] is null)
                        {
                            Pair[0] = net;
                            chance1 = int.MaxValue;
                            continue;
                        }

                        if (chance2 < n2Chance && Pair[1] is null)
                        {
                            Pair[1] = net;
                            chance2 = int.MaxValue;
                        }
                    }
                }
            }

            public Network[] CrossOver(int layers, int size)
            {
                if (Pair[1] is null || Pair[0] is null)
                {
                    return [];
                }

                List<Network> childs = [];
                var count = random.Next(1, 4);

                for (int i = 0; i < count; i++)
                {
                    childs.AddRange(Network.CrossOver(Pair[0], Pair[1], layers, size));
                }

                return [.. childs];
            }
        }

        private Generation CrossingOver()
        {
            var nets = new List<Network>();
            List<Network> listed = [.. networks];

            var counter = 0;

            while (counter++ < size)
            {
                var res = new NetworkPair(listed).CrossOver(layers, layerSize);
                if (res.Length > 0)
                {
                    nets.AddRange(res);
                }
            }

            Parallel.ForEach(nets, (net) =>
            {
                net.Score = 0;
            });

            Parallel.ForEach(nets, (network) =>
            {
                GameNeuro<Network> game = new(seed >> 3);
                for (int @try = 0; @try < tries; @try++)
                {
                    game.Play(network, false);
                    network.Score = (game.Ticks / 100) + (game.Points / 10);
                    game.Clear(network);
                }
            });

            var best = nets.OrderByDescending(x => x.Score).Take(size);
            Parallel.ForEach(best, (net) =>
            {
                net.Score = 0;
            });

            return new Generation(size, tries, layers, layerSize, ++generationNumber) { networks = [.. best] };
        }

        public Generation MakeNewGeneration()
        {
            PlayAndCountBestPoints();
            Console.WriteLine($"generation: {generationNumber}; " +
                              $"minimum points: {networks.Min(x => x.Score)} " +
                              $"average points: {networks.Average(x => (float)x.Score)} " +
                              $"maximum points: {networks.Max(x => x.Score)}; " +
                              $"overal maximum: {bestScore}");
            return CrossingOver();
        }
    }
}
