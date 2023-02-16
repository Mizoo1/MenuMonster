using System;
using System.Collections.Generic;
using SDL2;
using MenuMonster.Menu;

namespace MenuExample
{
    public class Program
    {

        public static void Main(string[] args)
        {
            MenuManager menuManager = new MenuManager();
            menuManager.StartGameLoop();
        }
       
    }
}
