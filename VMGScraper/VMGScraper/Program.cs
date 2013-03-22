using System;

namespace VMGScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var virginMoneyGivingScraper = new VirginMoneyGivingScraper();
            var myDonateScraper = new MyDonateScraper();
            try
            {
                if (args[0] == "vmg" || args[0] == "both")
                    virginMoneyGivingScraper.Scrape();
                if (args[0] == "mydonate" || args[0] == "both")
                    myDonateScraper.Scrape();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR! Please take a screenshot of this window and send to Mark G.");
                Console.WriteLine("Error Message: {0}",ex.Message);
                Console.WriteLine("StackTrace: {0}",ex.StackTrace);
                Console.ReadLine();
            }
        }
    }
}
