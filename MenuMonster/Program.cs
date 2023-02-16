using System;
using System.Collections.Generic;
using SDL2;
using MenuMonster.Menu;

namespace MenuExample
{
    public class Widget
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Index { get; set; }
        public string Label { get; set; }
        public Widget Prev { get; set; }
        public Widget Next { get; set; }
    }
    public class App
    {
        public Action Logic { get; set; }
        public Action Draw { get; set; }
        public IntPtr Renderer { get; set; }
        public IntPtr Window { get; set; }
        public bool[] Keyboard { get; set; }
        public double DeltaTime { get; set; }
        public Widget ActiveWidget { get; set; }
        public int FPS { get; set; }
    }

    public class Program
    {
        private static Widget widgetHead = new Widget();
        private static Widget widgetTail = widgetHead;
        private static App app = new App();
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
            app.Logic = Logic;
            app.Draw = Draw;
            then = SDL.SDL_GetTicks();
            long nextFPS = then + 1000;
            GameLoop();
        }
        public static void InitializeApp()
        {
            // Load the background image
            backgroundTexture = LoadTexture("Font/Monster_.jpg", app.Renderer);
            SDL_ttf.TTF_Init();
            fontPointer = SDL_ttf.TTF_OpenFont("Font/Lato-Regular.ttf", 24);
            if (fontPointer == IntPtr.Zero)
            {
                Console.WriteLine("Error opening font: " + SDL.SDL_GetError());
                return;
            }
            // Initialize the window and renderer here
            app.Window = SDL.SDL_CreateWindow("My Window",
                                      SDL.SDL_WINDOWPOS_CENTERED,
                                      SDL.SDL_WINDOWPOS_CENTERED,
                                      800, 600,
                                      SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            if (app.Window == IntPtr.Zero)
            {
                Console.WriteLine("Error creating window: " + SDL.SDL_GetError());
                return;
            }

            app.Renderer = SDL.SDL_CreateRenderer(app.Window, -1,
                                                     SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                                                     SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);
            if (app.Renderer == IntPtr.Zero)
            {
                Console.WriteLine("Error creating renderer: " + SDL.SDL_GetError());
                return;
            }
            backgroundTexture = LoadTexture("Font\\Monster_.jpg", app.Renderer);
            textDrawer = new TextDrawer(app.Renderer, fontPointer);
            InitWidgets();
            InitDemo();
        }

        public static void GameLoop()
        {
            while (true)
            {
                PrepareScene();
                DoInput();
                app.Logic();
                app.Draw();
                PresentScene();
                // allow the CPU/GPU to breathe
                SDL.SDL_Delay(1);

                app.DeltaTime = (SDL.SDL_GetTicks() - then) * LOGIC_RATE;
                DoFPS();
            }
        }

            private static void InitWidgets()
        {
            widgetHead = new Widget();
            widgetTail = widgetHead;
        }

        private static Widget CreateWidget(string name)
        {
            var w = new Widget
            {
                Name = name,
                Prev = widgetTail,
            };

            widgetTail.Next = w;
            widgetTail = w;

            return w;
        }

        private static void HandleKeyboardInput()
        {


            if (app.Keyboard == null)
            {
                app.Keyboard = new bool[(int)SDL.SDL_Scancode.SDL_NUM_SCANCODES];
            }
            if (app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP])
            {
                app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] = false;
                app.ActiveWidget = app.ActiveWidget.Prev;
                if (app.ActiveWidget == null)
                {
                    app.ActiveWidget = widgetTail;
                }
            }

            if (app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN])
            {
                app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN] = false;
                app.ActiveWidget = app.ActiveWidget.Next;
                if (app.ActiveWidget == null)
                {
                    app.ActiveWidget = widgetHead.Next;
                }
            }

            if (app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_F])
            {
                app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_F] = false;
                int flags = (int)SDL.SDL_GetWindowFlags(app.Window);
                if ((flags & (int)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP) == (int)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP)
                {
                    SDL.SDL_SetWindowFullscreen(app.Window, 0);
                }
                else
                {
                    SDL.SDL_SetWindowFullscreen(app.Window, (int)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
                }
            }
        }
        private static void HandleMenuNavigation()
        {
            if (app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] && app.ActiveWidget.Label == "Start")
            {
                app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] = false;
                Console.WriteLine("Start Menu");
            }
            if (app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] && app.ActiveWidget.Label == "Options")
            {
                app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] = false;
                DrawSubMenu();
            }
            if (app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] && app.ActiveWidget.Label == "Back")
            {
                app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] = false;
                InitWidgets();
                InitDemo();
            }
            if (app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] && app.ActiveWidget.Label == "Submenu Option 1")
            {
                app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] = false;
                DrawSubsubMenu();
                ClearAndDrawSubmenu(DrawSubsubMenu);
            }
            if (app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] && app.ActiveWidget.Label == "Back")
            {
                app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] = false;
                if (app.ActiveWidget.Index == 1)
                {
                    DrawSubMenu();
                }
                else if (app.ActiveWidget.Index == 2)
                {
                    ClearAndDrawSubmenu(DrawSubMenu);
                }
            }
            if (app.Keyboard[(int)SDL.SDL_Scancode.SDL_SCANCODE_RETURN] && app.ActiveWidget.Label == "Exit")
            {
                Environment.Exit(0);
            }
        }


        private static void DrawWidgets()
        {
            var w = widgetHead.Next;
            while (w != null)
            {
                if (w == app.ActiveWidget)
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

        private static void InitDemo()
        {
            var x = 500;

            var w = CreateWidget("start");
            w.X = x;
            w.Y = 200;
            w.Label = "Start";

            app.ActiveWidget = w;

            w = CreateWidget("load");
            w.X = x;
            w.Y = 250;
            w.Label = "Load";
            w = CreateWidget("options");
            w.X = x;
            w.Y = 300;
            w.Label = "Options";

            w = CreateWidget("credits");
            w.X = x;
            w.Y = 350;
            w.Label = "Credits";

            w = CreateWidget("exit");
            w.X = x;
            w.Y = 400;
            w.Label = "Exit";
        }
        private static void DrawSubMenu()
        {
            // Löschen Sie das Hauptmenü
            widgetTail = widgetHead;
            app.ActiveWidget = widgetTail;

            // Zeichnen Sie den Submenu
            var w = CreateWidget("submenu1");
            w.X = 100;
            w.Y = 200;
            w.Label = "Submenu Option 1";

            w = CreateWidget("submenu2");
            w.X = 100;
            w.Y = 250;
            w.Label = "Submenu Option 2";

            w = CreateWidget("submenu3");
            w.X = 100;
            w.Y = 300;
            w.Label = "Submenu Option 3";

            w = CreateWidget("back");
            w.X = 100;
            w.Y = 350;
            w.Label = "Back";
            w.Index = 1;
        }
        private static void DrawSubsubMenu()
        {
            widgetTail = widgetHead;
            app.ActiveWidget = widgetTail;
            var w = CreateWidget("subsubmenu1");
            w.X = 100;
            w.Y = 200;
            w.Label = "Subsubmenu Option 1";

            w = CreateWidget("subsubmenu2");
            w.X = 100;
            w.Y = 250;
            w.Label = "Subsubmenu Option 2";

            w = CreateWidget("subsubmenu3");
            w.X = 100;
            w.Y = 300;
            w.Label = "Back";
            w.Index = 2;
        }
        private static void ClearAndDrawSubmenu(Action drawSubmenu)
        {
            var w = widgetHead.Next;
            while (w != null)
            {
                var next = w.Next;
                w = next;
            }
            InitWidgets();
            drawSubmenu();
        }
        private static void Logic()
        {
            HandleKeyboardInput();
            HandleMenuNavigation();
        }

        private static void Draw()
        {
            SDL.SDL_RenderCopy(app.Renderer, backgroundTexture, IntPtr.Zero, IntPtr.Zero);
            DrawWidgets();
            
        }

        private static void PrepareScene()
        {
            
            SDL.SDL_SetRenderDrawColor(app.Renderer, 0, 0, 0, 255);
            SDL.SDL_RenderClear(app.Renderer);
            
            // Render the background image
            SDL.SDL_RenderCopy(app.Renderer, backgroundTexture, IntPtr.Zero, IntPtr.Zero);

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
                    app.Keyboard[(int)e.key.keysym.scancode] = true;
                }
                else if (e.type == SDL.SDL_EventType.SDL_KEYUP)
                {
                    app.Keyboard[(int)e.key.keysym.scancode] = false;
                }
            }
        }

        private static void PresentScene()
        {
            SDL.SDL_RenderPresent(app.Renderer);
        }

        private static void DoFPS()
        {
            long now = SDL.SDL_GetTicks();
            then = then == 0 ? now : then;
            app.DeltaTime = (now - then) / 1000.0;
            app.FPS = (int)(1.0 / app.DeltaTime);
            Console.WriteLine("FPS: " + app.FPS);
            then = now;
        }
    }
}
