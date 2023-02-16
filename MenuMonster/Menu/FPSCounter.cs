using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace MenuMonster.Menu
{
    public class FPSCounter
    {
        private static long then;
        public static void DoFPS()
        {
            long now = SDL.SDL_GetTicks();
            then = then == 0 ? now : then;
            Menu.app.DeltaTime = (now - then) / 1000.0;
            Menu.app.FPS = (int)(1.0 / Menu.app.DeltaTime);
            Console.WriteLine("FPS: " + Menu.app.FPS);
            then = now;
        }
    }
}
