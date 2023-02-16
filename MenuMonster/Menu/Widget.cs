using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuMonster.Menu
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
}
