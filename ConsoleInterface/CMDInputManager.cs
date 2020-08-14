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

        private readonly IList<string> quitCmds 
            = new List<string>{ "Quit", "quit", "Q", "q" };
        private const string helpCmd = "Help";
        private const string showCmd = "Show";
        private const string cropCmd = "Crops";
        private const string marketCmd = "Market";
        private const string marketsCmd = "Markets";
        private const string mineCmd = "Mines";
        private const string processCmd = "Processes";
        private const string currencyCmd = "Currencies";
        private const string popCmd = "Populations";
        private const string productCmd = "Products";
        private const string runMarketCmd = "Run";

        #endregion Commands

        public int PromptReader(ConsoleWorldManager worldManager)
        {
            this.worldManager = worldManager;
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
            var lineBreakdown = line.Split();

            switch (lineBreakdown[0])
            {
                case helpCmd:
                    PrintHelp();
                    break;
                case showCmd:
                    ShowOptions(lineBreakdown);
                    break;
                case runMarketCmd:
                    RunMarketCommands(lineBreakdown);
                    break;
                default:
                    quit = quitCheck(lineBreakdown[0]);
                    if (!quit)
                        Console.WriteLine("Invalid Command, try 'help'.");
                    break;
            }
        }

        private void RunMarketCommands(string[] lineBreakdown)
        {
            // get words and check it is correct.

            if (lineBreakdown.Count() != 4)
            {
                PrintRunHelp();
                return;
            }

            // If correct length, check the words
            var run = lineBreakdown[0];
            var ffor = lineBreakdown[1];
            var days = lineBreakdown[2];
            var dayName = lineBreakdown[4];
            if (run != "Run" || ffor != "for" || dayName != "Days")
            {
                PrintRunHelp();
                return;
            }
            int dayCount;
            if (int.TryParse(days, out dayCount))
            {
                PrintRunHelp();
                return;
            }

            if (dayCount <1)
            {
                PrintRunHelp();
                return;
            }

            // Everything is correct, so let's calculate.
            worldManager.RunFor(dayCount);
        }

        private void PrintRunHelp()
        {
            Console.WriteLine("Invalid Run command.\n" +
                "Command must come in the form 'Run for [#] Days'.\n" +
                "[#] must be a whole number larger than 0.");
        }

        private bool quitCheck(string command)
        {
            if (quitCmds.Contains(command))
                return true;
            return false;
        }

        private void ShowOptions(string[] lineBreakdown)
        {
            if (lineBreakdown.Count() <= 1)
                PrintShowHelp();

            var table = lineBreakdown[1];
            var market = "";

            if (lineBreakdown.Count() == 4)
            {
                market = lineBreakdown[3];
            }
            else if (lineBreakdown.Count() > 4)
            {
                market = string.Join(" ", lineBreakdown.Skip(3));
            }
            else if (lineBreakdown.Count() != 2)
            {
                Console.WriteLine("Invalid command. Command must be in form of 'show [Table] (in [Market])");
                return;
            }


            switch (lineBreakdown[1])
            {
                case cropCmd:
                    Console.WriteLine(worldManager.PrintCrops(market));
                    break;
                case marketCmd:
                    Console.WriteLine(worldManager.PrintMarketNames());
                    break;
                case marketsCmd:
                    Console.WriteLine(worldManager.PrintMarkets());
                    break;
                case mineCmd:
                    Console.WriteLine(worldManager.PrintMines(market));
                    break;
                case processCmd:
                    Console.WriteLine(worldManager.PrintProcesses(market));
                    break;
                case currencyCmd:
                    Console.WriteLine(worldManager.PrintCurrencies(market));
                    break;
                case popCmd:
                    Console.WriteLine(worldManager.PrintPops(market));
                    break;
                case productCmd:
                    Console.WriteLine(worldManager.PrintProducts(market));
                    break;
            }
        }

        private void PrintShowHelp()
        {
            var result = "Show [Table] (in [Market]) Command:\n\n" +
                "The Show command shows the data of the requested table.\n" +
                "Include 'in [Market]' to show the data for a specific market.\n" +
                "To use replace [Table] with other one of these possible options.\n" +
                "- Crops\n" + // done
                "- Mines\n" + // done
                "- Processes\n" + // done
                "- Populations\n" + // done
                "- Products\n" + // done
                "- Currencies\n" + // done
                "- Markets"; // Done
            Console.WriteLine(result);
        }

        private void PrintHelp()
        {
            var result = "Available Commands:\n" +
                "\t Show [Table]: Shows the requested [Table] of items\n" +
                "\t Run for [number] days: Runs the markets for the number of days given.\n" +
                "\t\t Must give a whole number greater than 0.\n" +
                "\t help: shows available commands\n" +
                "\t quit/Quit/Q/q: Exits the program.\n";
            Console.Write(result);
        }
    }
}

