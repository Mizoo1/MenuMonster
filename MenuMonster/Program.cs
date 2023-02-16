using System;
using System.Collections.Generic;
using SDL2;

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
        public static void Main(string[] args)
        {
            SDL_ttf.TTF_Init();

            fontPointer = SDL_ttf.TTF_OpenFont("Font/Lato-Regular.ttf", 24);
            if (fontPointer == IntPtr.Zero)
            {
                Console.WriteLine("Error opening font: " + SDL.SDL_GetError());
                return;
            }
            // Initialisiere das Fenster und den Renderer hier
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
            InitWidgets();
            InitDemo();
            app.Logic = Logic;
            app.Draw = Draw;
            then = SDL.SDL_GetTicks();
            long nextFPS = then + 1000;
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

        private static void DoWidgets()
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
                }else if (app.ActiveWidget.Index == 2)
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
                    DrawText(">" + "  " + w.Label, w.X - 40, w.Y, 0, 255, 0, 500, 10);
                }
                else
                {
                    DrawText(w.Label, w.X, w.Y, 255, 255, 255, 500, 10);
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
            //drawSubmenu();
        }
        private static void Logic()
        {
            DoWidgets();
        }

        private static void Draw()
        {
            DrawWidgets();
        }

        private static void PrepareScene()
        {
            SDL.SDL_SetRenderDrawColor(app.Renderer, 0, 0, 0, 255);
            SDL.SDL_RenderClear(app.Renderer);
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
            //Console.WriteLine("FPS: " + app.FPS);
            then = now;
        }

        private static void DrawText(string text, int x, int y, int r, int g, int b, int align, int font)
        {




            // create a surface from the text
            SDL.SDL_Color color = new SDL.SDL_Color() { r = (byte)r, g = (byte)g, b = (byte)b };
            IntPtr textSurface = SDL_ttf.TTF_RenderText_Blended(fontPointer, text, color);
            if (textSurface == IntPtr.Zero)
            {
                Console.WriteLine("Error creating text surface: " + SDL.SDL_GetError());
                return;
            }

            // create a texture from the surface
            IntPtr textTexture = SDL.SDL_CreateTextureFromSurface(app.Renderer, textSurface);
            if (textTexture == IntPtr.Zero)
            {
                Console.WriteLine("Error creating texture: " + SDL.SDL_GetError());
                return;
            }

            // get the width and height of the text
            int w, h;
            SDL.SDL_QueryTexture(textTexture, out uint format, out int access, out w, out h);
            // calculate the position based on the text alignment
            SDL.SDL_Rect textRect = new SDL.SDL_Rect();
            if (align == TEXT_ALIGN_RIGHT)
            {
                textRect.x = x - w;
            }
            else if (align == TEXT_ALIGN_CENTER)
            {
                textRect.x = x - w / 2;
            }
            else
            {
                textRect.x = x;
            }
            textRect.y = y;
            textRect.w = w;
            textRect.h = h;

            // render the text to the screen
            SDL.SDL_RenderCopy(app.Renderer, textTexture, IntPtr.Zero, ref textRect);

            // free the surface and texture
            SDL.SDL_FreeSurface(textSurface);
            SDL.SDL_DestroyTexture(textTexture);
        }
    }
}
