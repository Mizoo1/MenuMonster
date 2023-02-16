using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace MenuMonster.Menu
{
    public class AppInitializer
    {
        private static IntPtr fontPointer;
        public  static TextDrawer textDrawer;
        public static IntPtr backgroundTexture;

        public static void InitializeApp()
        {
            InitializeFont();
            InitializeWindowAndRenderer();
            InitializeWidgetsAndDemo();
        }

        private static void InitializeFont()
        {
            SDL_ttf.TTF_Init();
            fontPointer = SDL_ttf.TTF_OpenFont("Font/Lato-Regular.ttf", 24);
            if (fontPointer == IntPtr.Zero)
            {
                Console.WriteLine("Error opening font: " + SDL.SDL_GetError());
                return;
            }
            
        }

        private static void InitializeWindowAndRenderer()
        {
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
        }

        private static void InitializeWidgetsAndDemo()
        {
            textDrawer = new TextDrawer(Menu.app.Renderer, fontPointer);
            Menu.InitWidgets();
            Menu.InitDemo();
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
    }

}
