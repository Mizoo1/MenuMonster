using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace MenuMonster.Menu
{
    public class TextDrawer
    {
        private IntPtr renderer;
        private IntPtr fontPointer;
        public TextDrawer(IntPtr renderer, IntPtr fontPointer)
        {
            this.renderer = renderer;
            this.fontPointer = fontPointer;
        }

        public void DrawText(string text, int x, int y, int r, int g, int b, int align, int value_right, int value_center)
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
            IntPtr textTexture = SDL.SDL_CreateTextureFromSurface(renderer, textSurface);
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
            if (align == value_right)
            {
                textRect.x = x - w;
            }
            else if (align == value_center)
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
            SDL.SDL_RenderCopy(renderer, textTexture, IntPtr.Zero, ref textRect);

            // free the surface and texture
            SDL.SDL_FreeSurface(textSurface);
            SDL.SDL_DestroyTexture(textTexture);
        }
    }
}
