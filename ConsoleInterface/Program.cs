using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleInterface
{
    class Program
    {
        private static CMDInputManager inputManager;
        static void Main(string[] args)
        {
            inputManager = new CMDInputManager();
            // Initial Loading
            Console.WriteLine("Loading Values ----");
            var worldManager = new ConsoleWorldManager();
            worldManager.Open();
            try
            {
                worldManager.LoadDB();
                // Finished Loading
                Console.WriteLine("Loaded ----");

                // Activate Promt manager.
                inputManager.PromptReader(worldManager);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                worldManager.Close();
            }
        }
    }
}
