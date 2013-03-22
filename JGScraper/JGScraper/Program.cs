using System;

namespace JGScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter 'vmg', 'mydonate' or 'both'");
            var choice = Console.ReadLine();
            var virginMoneyGivingScraper = new VirginMoneyGivingScraper();
            var myDonateScraper = new MyDonateScraper();
            try
            {
                if (choice == "vmg" || choice == "both")
                    virginMoneyGivingScraper.Scrape();
                if (choice == "mydonate" || choice == "both")
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
