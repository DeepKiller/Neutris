using System.Xml.Serialization;
using Neutris.Game;
using Neutris.Neuro;
using Neutris.Neuro.Common;
using Neutris.Neuro.FNN;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Neutris.Graphic
{
    internal class DisplayWindow<Tnet> : GameWindow where Tnet : INetwork
    {
        readonly GameNeuro<Tnet> Game;

        static Vector2i? lastLocation;

        public DisplayWindow(int width, int height, string title, GameNeuro<Tnet> gameNeuro)
            :
            base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title, Profile = ContextProfile.Compatability, Location = lastLocation, StartFocused = false })
        {
            Game = gameNeuro;

            VSync = VSyncMode.On;
            UpdateFrequency = 60;
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        private bool drawn = false;

        protected override void OnMove(WindowPositionEventArgs e)
        {
            base.OnMove(e);
            if (drawn)
            {
                lastLocation = e.Position;
            }
        }

        private Vector3d ConvertCoordinate(long x, long y)
        {
            var halfWidth = ClientSize.X / 2d;
            var halfHeight = ClientSize.Y / 2d;

            var vX = (x / halfWidth) - 1d;
            var vY = -((y / halfHeight) - 1d);

            return new Vector3d(vX, vY, 0);
        }

        private Vector3 PickColor(int value)
        {
            return value switch
            {
                0 => new Vector3(1f, 1f, 1f),
                1 => new Vector3(1f, 1f, 0f),
                2 => new Vector3(1f, 0.5f, 0f),
                3 => new Vector3(0f, 0f, 1f),
                4 => new Vector3(0.5f, 0f, 0.5f),
                5 => new Vector3(0f, 1f, 1f),
                6 => new Vector3(0f, 1f, 0f),
                7 => new Vector3(1f, 0f, 0f),
                8 => new Vector3(1f, 0.3f, 0f),
                _ => new Vector3(0, 0, 0)
            };
        }

        private void DrawRectangle(long x, long y, long width, long height, int value)
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(PickColor(value));
            GL.Vertex3(ConvertCoordinate(x, y));
            GL.Vertex3(ConvertCoordinate(x, y + height));
            GL.Vertex3(ConvertCoordinate(x + width, y + height));
            GL.Vertex3(ConvertCoordinate(x + width, y));
            GL.End();

            if (value > 0)
            {
                GL.Begin(PrimitiveType.Lines);
                GL.Color3(0.5, 0.5, 0.5);
                GL.Vertex3(ConvertCoordinate(x, y));
                GL.Vertex3(ConvertCoordinate(x + width, y));
                GL.Vertex3(ConvertCoordinate(x + width, y));
                GL.Vertex3(ConvertCoordinate(x + width, y + height));
                GL.Vertex3(ConvertCoordinate(x + width, y + height));
                GL.Vertex3(ConvertCoordinate(x, y + height));
                GL.Vertex3(ConvertCoordinate(x, y + height));
                GL.Vertex3(ConvertCoordinate(x, y));
                GL.End();
            }
        }

        private void DrawPoint(long x, long y, long diametr, bool isActive)
        {
            GL.PointSize(diametr);
            GL.Enable(EnableCap.PointSmooth);
            GL.Begin(PrimitiveType.Points);
            if (isActive)
            {
                GL.Color3(0f, 0.8f, 0f);
            }
            else
            {
                GL.Color3(0.8f, 0f, 0f);
            }
            GL.Vertex3(ConvertCoordinate(x, y));
            GL.End();
            GL.Disable(EnableCap.PointSmooth);
            GL.PointSize(1);
        }

        private void DrawLine(long x, long y, long toX, long toY, bool isActive)
        {
            GL.Begin(PrimitiveType.Lines);
            if (isActive)
            {
                GL.Color3(0f, 0.8f, 0f);
            }
            else
            {
                GL.Color3(0.8f, 0f, 0f);
            }
            GL.Vertex3(ConvertCoordinate(x, y));
            GL.Vertex3(ConvertCoordinate(toX, toY));
            GL.End();
        }

        private void ConvertLayersToArray(ref Neuron?[,] neurons, Network network)
        {
            var vertSize = neurons.GetLength(1);
            var horSize = neurons.GetLength(0);

            for (int x = 0; x < horSize; x++)
            {
                Layer layer;

                if (x == 0)
                {
                    layer = network.InputLayer;
                }
                else if (x == horSize - 1)
                {
                    layer = network.OutputLayer;
                }
                else
                {
                    layer = network.Layers[x - 1];
                }

                var min = vertSize - layer.Neurons.Length;
                var differ = min / 2;
                var differOpposite = (min % 2 == 0 ? min : min + 1) / 2;

                for (int y = 0; y < vertSize; y++)
                {
                    if (y >= differ && y < vertSize - differOpposite)
                    {
                        neurons[x, y] = layer.Neurons[y - differ];
                    }
                }
            }
        }

        private void DrawNetwork(long x, long y, long width, long height)
        {
            if (Game.Network is Network network)
            {
                DrawRectangle(x, y, width, height, 0);
                var horizontalMaxSize = network.Layers.Count + 2;
                var verticalMaxSize = Math.Max(network.InputLayer.Neurons.Length, Math.Max(network.OutputLayer.Neurons.Length, network.Layers.Max(x => x.Neurons.Length)));

                var doubledHorizontal = horizontalMaxSize * 2 + 1;
                var doubledVertical = verticalMaxSize * 2 + 1;

                var vertStep = height / doubledVertical;
                var horStep = width / doubledHorizontal;

                var diametr = Math.Min(horStep, vertStep);

                var neurons = new Neuron?[horizontalMaxSize, verticalMaxSize];

                ConvertLayersToArray(ref neurons, network);

                for (int xn = 0; xn < neurons.GetLength(0); xn++)
                {
                    for (int yn = 0; yn < neurons.GetLength(1); yn++)
                    {
                        if (neurons[xn, yn] is Neuron neuron)
                        {
                            var xcoord = xn + diametr + xn * horStep * 2;
                            var ycoord = yn + (diametr / 2) + yn * vertStep * 2;
                            DrawPoint(x + xcoord, ycoord, diametr, neuron.IsActive());
                        }
                    }
                }
            }
        }

        bool paused = false;

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space)
            {
                paused = !paused;
            }

            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.W)
            {
                UpdateFrequency += 10;
            }

            if (e.Key == OpenTK.Windowing.GraphicsLibraryFramework.Keys.S)
            {
                UpdateFrequency -= 10;
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (!paused)
            {
                if (Game.UpdateVisual())
                {
                    Close();
                }
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            drawn = true;
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            var width = ClientSize.X;
            var height = ClientSize.Y;

            var vertSize = height / (Field.HEIGHT + 2);
            var horSize = width / (Field.WIDTH + 2);

            var size = Math.Min(vertSize, horSize);

            for (int i = 0; i < Field.WIDTH + 2; i++)
            {
                DrawRectangle(i * size, 0, size, size, 8);
            }

            var map = Game.Map;

            for (int y = 0; y < Field.HEIGHT; y++)
            {
                DrawRectangle(0, (y + 1) * size, size, size, 8);
                for (int x = 0; x < Field.WIDTH; x++)
                {
                    DrawRectangle((x + 1) * size, (y + 1) * size, size, size, map[x, y]);
                }
                DrawRectangle((Field.WIDTH + 1) * size, (y + 1) * size, size, size, 8);
            }

            for (int i = 0; i < Field.WIDTH + 2; i++)
            {
                DrawRectangle(i * size, (Field.HEIGHT + 1) * size, size, size, 8);
            }

            DrawNetwork(size * (Field.WIDTH + 2), 0, width - (Field.WIDTH + 2) * size, size * (Field.HEIGHT + 2));

            SwapBuffers();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Game.PrepareVisual();

            GL.ClearColor(0f, 0f, 0f, 1f);
        }
    }
}
