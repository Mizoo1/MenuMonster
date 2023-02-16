using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace MenuMonster.Menu
{
    public class InputHandler
    {
        public static void HandleInput()
        {
            Logic();
        }

        private static void HandleKeyboardInput()
        {
            if (Menu.app.Keyboard == null)
            {
                Menu.app.Keyboard = new bool[(int)SDL.SDL_Scancode.SDL_NUM_SCANCODES];
            }
            if (Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP])
            {
                Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] = false;
                Menu.app.ActiveWidget = Menu.app.ActiveWidget.Prev;
                if (Menu.app.ActiveWidget == null)
                {
                    Menu.app.ActiveWidget = Menu.widgetTail;
                }
            }

            if (Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN])
            {
                Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN] = false;
                Menu.app.ActiveWidget = Menu.app.ActiveWidget.Next;
                if (Menu.app.ActiveWidget == null)
                {
                    Menu.app.ActiveWidget = Menu.widgetHead.Next;
                }
            }

            if (Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_F])
            {
                Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_F] = false;
                int flags = (int)SDL.SDL_GetWindowFlags(Menu.app.Window);
                if ((flags & (int)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP) == (int)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP)
                {
                    SDL.SDL_SetWindowFullscreen(Menu.app.Window, 0);
                }
                else
                {
                    SDL.SDL_SetWindowFullscreen(Menu.app.Window, (int)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
                }
            }
        }
        private static void HandleMenuNavigation()
        {
            if (Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] && Menu.app.ActiveWidget.Label == "Start")
            {
                Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] = false;
                Console.WriteLine("Start Menu");
            }
            if (Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] && Menu.app.ActiveWidget.Label == "Options")
            {
                Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] = false;
                Menu.DrawSubMenu();
            }
            if (Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] && Menu.app.ActiveWidget.Label == "Back")
            {
                Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] = false;
                Menu.InitWidgets();
                Menu.InitDemo();
            }
            if (Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] && Menu.app.ActiveWidget.Label == "Submenu Option 1")
            {
                Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] = false;
                Menu.DrawSubsubMenu();
                Menu.ClearAndDrawSubmenu(Menu.DrawSubsubMenu);
            }
            if (Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] && Menu.app.ActiveWidget.Label == "Back")
            {
                Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] = false;
                if (Menu.app.ActiveWidget.Index == 1)
                {
                    Menu.DrawSubMenu();
                }
                else if (Menu.app.ActiveWidget.Index == 2)
                {
                    Menu.ClearAndDrawSubmenu(Menu.DrawSubMenu);
                }
            }
            if (Menu.app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] && Menu.app.ActiveWidget.Label == "Exit")
            {
                Environment.Exit(0);
            }
        }
        public  static void Logic()
        {
            HandleKeyboardInput();
            HandleMenuNavigation();
        }
        public static void DoInput()
        {
            SDL.SDL_Event e;
            while (SDL.SDL_PollEvent(out e) != 0)
            {
                if (e.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    Environment.Exit(0);
                }
                else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                {
                    Menu.app.Keyboard[(int)e.key.keysym.scancode] = true;
                }
                else if (e.type == SDL.SDL_EventType.SDL_KEYUP)
                {
                    Menu.app.Keyboard[(int)e.key.keysym.scancode] = false;
                }
            }
        }
    }
}
