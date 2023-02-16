using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace MenuMonster.Menu
{
    
    public class MenuManager
    {
        private static readonly double LOGIC_RATE = 0.01;
        public static IntPtr fontPointer;
        public static long then;

        public MenuManager()
        {
            AppInitializer.InitializeApp();
            Menu.app.Logic = InputHandler.Logic;
            Menu.app.Draw = SceneRenderer.Draw;
            then = SDL.SDL_GetTicks();
        }

        public void StartGameLoop()
        {
            GameLoop();
        }

        private void GameLoop()
        {
            long nextFPS = then + 1000;
            while (true)
            {
                SceneRenderer.PrepareScene();
                InputHandler.DoInput();
                Menu.app.Logic();
                Menu.app.Draw();
                SceneRenderer.PresentScene();
                // allow the CPU/GPU to breathe
                SDL.SDL_Delay(1);
                Menu.app.DeltaTime = (SDL.SDL_GetTicks() - then) * LOGIC_RATE;
                FPSCounter.DoFPS();
            }
        }
    }

}
