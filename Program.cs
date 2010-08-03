using System;

namespace XtremeSweeper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (cXtremeSweeper_Game game = new cXtremeSweeper_Game())
            {
                game.Run();
            }
        }
    }
}

