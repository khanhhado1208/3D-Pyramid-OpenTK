using System;
using OpenTK.Windowing.Desktop;

class Program
{
    static void Main()
    {
        using (Game game = new Game())
        {
            game.Run(); // Run OpenGL
        }
    }
}
