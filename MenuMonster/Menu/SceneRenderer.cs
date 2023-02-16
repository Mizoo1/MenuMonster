using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace MenuMonster.Menu
{
    public class SceneRenderer
    {
        private static readonly int TEXT_ALIGN_RIGHT = 10;
        private static readonly int TEXT_ALIGN_CENTER = 50;

        public static void PrepareScene()
        {
            SDL.SDL_SetRenderDrawColor(Menu.app.Renderer, 0, 0, 0, 255);
            SDL.SDL_RenderClear(Menu.app.Renderer);

            SDL.SDL_RenderCopy(Menu.app.Renderer, AppInitializer.backgroundTexture, IntPtr.Zero, IntPtr.Zero);
        }

        public static void DrawWidgets()
        {
            var w = Menu.widgetHead.Next;
            while (w != null)
            {
                if (w == Menu.app.ActiveWidget)
                {
                    AppInitializer.textDrawer.DrawText(">" + "  " + w.Label, w.X - 40, w.Y, 0, 255, 0, 500, TEXT_ALIGN_RIGHT, TEXT_ALIGN_CENTER);
                }
                else
                {
                    AppInitializer.textDrawer.DrawText(w.Label, w.X, w.Y, 255, 255, 255, 500, TEXT_ALIGN_RIGHT, TEXT_ALIGN_CENTER);
                }

                w = w.Next;
            }
        }

        public static void Draw()
        {
            PrepareScene();
            DrawWidgets();
            PresentScene();
        }

        public static void PresentScene()
        {
            SDL.SDL_RenderPresent(Menu.app.Renderer);
        }
    }

}
