using System;
using System.Collections.Generic;

namespace MinerFusionConsole.Views
{
    public class FileView
    {
        public void DisplayErrors(IEnumerable<string> errors)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            foreach (var error in errors)
                Console.WriteLine(error);

            Console.ResetColor();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
