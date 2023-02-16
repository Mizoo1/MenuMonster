using System;
using System.Collections.Generic;
using SDL2;
using MenuMonster.Menu;

namespace MenuExample
{



    public class Program
    {

        private static readonly double LOGIC_RATE = 0.01;
        private const int TEXT_ALIGN_RIGHT = 10;
        private const int TEXT_ALIGN_CENTER = 50;
        public static IntPtr fontPointer;
        public static long then;
        private static IntPtr backgroundTexture;
        private static TextDrawer textDrawer;
        public static void Main(string[] args)
        {

            InitializeApp();
            Menu.app.Logic = Logic;
            Menu.app.Draw = Draw;
            then = SDL.SDL_GetTicks();
            long nextFPS = then + 1000;
            GameLoop();
        }
        public static void InitializeApp()
        {
            // Load the background image
            backgroundTexture = LoadTexture("Font/Monster_.jpg", Menu.app.Renderer);
            SDL_ttf.TTF_Init();
            fontPointer = SDL_ttf.TTF_OpenFont("Font/Lato-Regular.ttf", 24);
            if (fontPointer == IntPtr.Zero)
            {
                Console.WriteLine("Error opening font: " + SDL.SDL_GetError());
                return;
            }
            // Initialize the window and renderer here
            Menu.app.Window = SDL.SDL_CreateWindow("My Window",
                                      SDL.SDL_WINDOWPOS_CENTERED,
                                      SDL.SDL_WINDOWPOS_CENTERED,
                                      800, 600,
                                      SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            if (Menu.app.Window == IntPtr.Zero)
            {
                Console.WriteLine("Error creating window: " + SDL.SDL_GetError());
                return;
            }

            Menu.app.Renderer = SDL.SDL_CreateRenderer(Menu.app.Window, -1,
                                                     SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                                                     SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
            if (Menu.app.Renderer == IntPtr.Zero)
            {
                Console.WriteLine("Error creating renderer: " + SDL.SDL_GetError());
                return;
            }
            backgroundTexture = LoadTexture("Font\\Monster_.jpg", Menu.app.Renderer);
            textDrawer = new TextDrawer(Menu.app.Renderer, fontPointer);
            Menu.InitWidgets();
            Menu.InitDemo();
        }

        public static void GameLoop()
        {
            while (true)
            {
                PrepareScene();
                DoInput();
                Menu.app.Logic();
                Menu.app.Draw();
                PresentScene();
                // allow the CPU/GPU to breathe
                SDL.SDL_Delay(1);

                Menu.app.DeltaTime = (SDL.SDL_GetTicks() - then) * LOGIC_RATE;
                DoFPS();
            }
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


        private static void DrawWidgets()
        {
            var w = Menu.widgetHead.Next;
            while (w != null)
            {
                if (w == Menu.app.ActiveWidget)
                {
                    textDrawer.DrawText(">" + "  " + w.Label, w.X - 40, w.Y, 0, 255, 0, 500, TEXT_ALIGN_RIGHT, TEXT_ALIGN_CENTER);
                }
                else
                {
                    textDrawer.DrawText(w.Label, w.X, w.Y, 255, 255, 255, 500, TEXT_ALIGN_RIGHT, TEXT_ALIGN_CENTER);
                }

                w = w.Next;
            }
        }

        private static void Logic()
        {
            HandleKeyboardInput();
            HandleMenuNavigation();
        }

        private static void Draw()
        {
            SDL.SDL_RenderCopy(Menu.app.Renderer, backgroundTexture, IntPtr.Zero, IntPtr.Zero);
            DrawWidgets();
            
        }

        private static void PrepareScene()
        {
            
            SDL.SDL_SetRenderDrawColor(Menu.app.Renderer, 0, 0, 0, 255);
            SDL.SDL_RenderClear(Menu.app.Renderer);
            
            // Render the background image
            SDL.SDL_RenderCopy(Menu.app.Renderer, backgroundTexture, IntPtr.Zero, IntPtr.Zero);

        }
        private static IntPtr LoadTexture(string file, IntPtr renderer)
        {
            IntPtr texture = IntPtr.Zero;
            IntPtr surface = SDL_image.IMG_Load(file);
            if (surface == IntPtr.Zero)
            {
                Console.WriteLine("Error loading image file: " + SDL.SDL_GetError());
                return IntPtr.Zero;
            }
            texture = SDL.SDL_CreateTextureFromSurface(renderer, surface);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine("Error creating texture: " + SDL.SDL_GetError());
            }
            SDL.SDL_SetTextureBlendMode(texture, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
            SDL.SDL_FreeSurface(surface);
            return texture;
        }
        private static void DoInput()
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

        private static void PresentScene()
        {
            SDL.SDL_RenderPresent(Menu.app.Renderer);
        }

        private static void DoFPS()
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
