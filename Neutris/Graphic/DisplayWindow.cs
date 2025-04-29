using Neutris.Game;
using Neutris.Neuro;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Neutris.Graphic
{
    internal class DisplayWindow : GameWindow
    {
        readonly GameNeuro Game;

        static Vector2i? lastLocation;

        public DisplayWindow(int width, int height, string title, GameNeuro gameNeuro)
            :
            base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title, Profile = ContextProfile.Compatability, Location = lastLocation })
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

        protected override void OnMove(WindowPositionEventArgs e)
        {
            base.OnMove(e);

            lastLocation = e.Position;
        }

        private Vector3d ConvertCoordinate(long x, long y)
        {
            var halfWidth = ClientSize.X / 2d;
            var halfHeight = ClientSize.Y / 2d;

            var vX = (x / halfWidth) - 1d;
            var vY = -((y / halfHeight) - 1d);

            return new Vector3d(vX, vY, 0);
        }

        private void DrawRectangle(long x, long y, long width, long height, bool isEmpty)
        {
            GL.Begin(PrimitiveType.Quads);
            if (isEmpty)
            {
                GL.Color3(1d, 1d, 1d);
            }
            else
            {
                GL.Color3(1d, 0.3d, 0d);
            }
            GL.Vertex3(ConvertCoordinate(x, y));
            GL.Vertex3(ConvertCoordinate(x, y + height));
            GL.Vertex3(ConvertCoordinate(x + width, y + height));
            GL.Vertex3(ConvertCoordinate(x + width, y));
            GL.End();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (Game.UpdateVisual())
            {
                Close();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            var width = ClientSize.X;
            var height = ClientSize.Y;

            var vertSize = height / (Field.HEIGHT + 2);
            var horSize = width / (Field.WIDTH + 2);

            var size = Math.Min(vertSize, horSize);

            for (int i = 0; i < Field.WIDTH + 2; i++)
            {
                DrawRectangle(i * size, 0, size, size, false);
            }

            var map = Game.Map;

            for (int y = 0; y < Field.HEIGHT; y++)
            {
                DrawRectangle(0, (y + 1) * size, size, size, false);
                for (int x = 0; x < Field.WIDTH; x++)
                {
                    DrawRectangle((x + 1) * size, (y + 1) * size, size, size, !map[x, y]);
                }
                DrawRectangle((Field.WIDTH + 1) * size, (y + 1) * size, size, size, false);
            }

            for (int i = 0; i < Field.WIDTH + 2; i++)
            {
                DrawRectangle(i * size, (Field.HEIGHT + 1) * size, size, size, false);
            }

            SwapBuffers();
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Game.PrepareVisual();

            GL.ClearColor(1f, 1f, 1f, 1f);
        }
    }
}
