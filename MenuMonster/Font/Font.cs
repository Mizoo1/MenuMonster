using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

namespace Monster.Font
{
    public class Font
    {
        private IntPtr _font;
        private string _fontPath;
        private int _fontSize;

        public Font(string fontPath, int fontSize)
        {
            _fontPath = fontPath;
            _fontSize = fontSize;
        }

        public void Load()
        {
            // Make sure the font file exists
            if (!File.Exists(_fontPath))
            {
                throw new FileNotFoundException("Font file not found: " + _fontPath);
            }

            // Load the font
            _font = SDL_ttf.TTF_OpenFont(_fontPath, _fontSize);
            if (_font == IntPtr.Zero)
            {
                throw new Exception("Error loading font: " + SDL_ttf.TTF_GetError());
            }
        }

        public IntPtr RenderText(string text, SDL.SDL_Color color)
        {
            // Render the text to a surface
            IntPtr surface = SDL_ttf.TTF_RenderUTF8_Blended(_font, text, color);
            if (surface == IntPtr.Zero)
            {
                throw new Exception("Error rendering text: " + SDL_ttf.TTF_GetError());
            }

            return surface;
        }

        public void Dispose()
        {
            SDL_ttf.TTF_CloseFont(_font);
            _font = IntPtr.Zero;
        }
    }
}
