using System;

namespace VMGScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            var scraper = new VirginMoneyGivingScraper();
            try
            {
                scraper.Scrape();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}",ex.Message);
            }
            
        }
    }
}
