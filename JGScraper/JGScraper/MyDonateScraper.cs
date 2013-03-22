using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace JGScraper
{
    internal class MyDonateScraper
    {
        private readonly List<string> _charities = new List<string>();
        private const string _domain = "https://mydonate.bt.com";

        public void Scrape()
        {
            var timeStarted = DateTime.Now;
            var filePathAndName = Environment.CurrentDirectory + "\\charities-mydonate.csv";
            Console.WriteLine("MyDonate scraping started at {0}", DateTime.Now.ToString("HH:MM:ss"));
            File.WriteAllText(filePathAndName, "MyDonate scraper run on " + DateTime.Now.ToString("dd MMM yyyy") + Environment.NewLine);

            var request = new HtmlWeb();
            var pageUrl = "https://mydonate.bt.com/charity/charityIndex.html?ItemsToDisplay=50";

            while (!string.IsNullOrEmpty(pageUrl))
            {
                HtmlDocument page = request.Load(pageUrl);

                var charityNameElements = page.DocumentNode.Descendants().Where(element => element.GetAttributeValue("name", string.Empty) == "charityName");
                _charities.Add("CharityName, RegNumber");
                foreach (var charityNameElement in charityNameElements)
                {

                    var charityName = HttpUtility.HtmlDecode(charityNameElement.GetAttributeValue("value", string.Empty)).Trim(' ').Trim('\'');
                    var charityRegNumber = charityNameElement.NextSibling.NextSibling.GetAttributeValue("value", string.Empty);
                    _charities.Add(charityName + "," + charityRegNumber);
                    Console.WriteLine("Added charity: " + charityName + " - " + charityRegNumber);
                }

                var nextPageLink = page.DocumentNode.Descendants().Where(elem => elem.Name == "a" && elem.InnerText.Contains("Next")).FirstOrDefault();
                if (nextPageLink == null)
                    pageUrl = string.Empty;
                else
                    pageUrl = _domain + nextPageLink.GetAttributeValue("href", string.Empty) + "&ItemsToDisplay=50";
            }

            File.AppendAllLines(Environment.CurrentDirectory + "\\charities-mydonate.csv",_charities);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Total MyDonate charities scraped: {0}", _charities.Count-1);
            Console.WriteLine("charities-mydonate.csv file saved. Total MyDonate scraper running duration: " + (DateTime.Now - timeStarted).ToString());
            Console.WriteLine("Press Enter to continue.");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ReadLine();
        }
    }
}