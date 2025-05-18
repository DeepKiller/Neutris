using Neutris.Game;
using Neutris.Graphic;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Neutris.Neuro.FNN;

namespace Neutris
{
    public class Program
    {
        public static void Main()
        {

            var counter = 0;

            Generation generation = new(500, 1, 3, 50);

            while (counter <= 10000)
            {
                counter++;
                generation = generation.MakeNewGeneration();
            }

            //var window = new DisplayWindow(500, 500, "game");
            //window.Run();

            //var game = new Game.Game();
            //game.Play();
        }
    }
}