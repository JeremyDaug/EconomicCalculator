using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleInterface
{
    public class CMDInputManager
    {
        private string line;
        private bool quit = false;
        private ConsoleWorldManager worldManager;

        #region Commands

        private const string quitCmd = "quit";
        private const string helpCmd = "help";
        private const string showCmd = "show";

        #endregion Commands

        public int PromptReader(ConsoleWorldManager worldManager)
        {
            while (!quit)
            {
                Console.Write(">>>");
                line = Console.ReadLine();
                ProcessLine(line);
            }

            return 0;
        }

        private void ProcessLine(string line)
        {
            var stdLine = line.ToLower();
            var lineBreakdown = stdLine.Split();

            switch (lineBreakdown[0])
            {
                case quitCmd:
                    quit = true;
                    break;
                case helpCmd:
                    PrintHelp();
                    break;
                case showCmd:
                    ShowOptions(lineBreakdown);
                    break;
                default:
                    Console.WriteLine("Invalid Command, try 'help'.");
                    break;
            }
        }

        private void ShowOptions(string[] lineBreakdown)
        {
            if (lineBreakdown.Count() <= 1)
                PrintShowHelp();
        }

        private void PrintShowHelp()
        {
            var result = "Show [Table] Command:\n\n" +
                "The Show command shows the data of the requested table.\n" +
                "To use replace [Table] with other one of these possible options.\n" +
                "- Crops\n" +
                "- Mines\n" +
                "- Processes\n" +
                "- Populations\n" +
                "- Products\n" +
                "- Currencies\n" +
                "- Markets";
            Console.WriteLine(result);
        }

        private void PrintHelp()
        {
            var result = "Available Commands:\n" +
                "\t Show [Table]: Shows the requested [Table] of items\n" +
                "\t help: shows available commands\n" +
                "\t quit: Exits the program.\n";
            Console.Write(result);
        }
    }
}
