using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuMonster.Menu
{
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
}
