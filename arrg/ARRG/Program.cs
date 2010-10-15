using System;

namespace ARRG_Game
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ARRG game = new ARRG())
            {
                game.Run();
            }
        }
    }
}

