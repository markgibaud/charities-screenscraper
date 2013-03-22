using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using HtmlAgilityPack;

namespace JGScraper
{
    internal class VirginMoneyGivingScraper : IScraper
    {
        private readonly List<string> _linksToCategoryPages = new List<string>();
        private readonly List<string> _linksToCharityListPages = new List<string>();
        private readonly List<string> _charities = new List<string>();

        private const string domain = "http://uk.virginmoneygiving.com";
        private CookieCollection _cookieCollection;

        public void Scrape()
        {
            var timeStarted = DateTime.Now;
            Console.WriteLine("VMG scraping started at {0}",DateTime.Now.ToString("HH:MM:ss"));

            _charities.Add("CharityName, RegNumber");

            ScrapeCategoriesListPage();
            ScrapeSubcategoriesListPages();
            ScrapeCharitiesListPage();

            var distinctCharityNames = _charities.Distinct();

            Console.WriteLine("Total VMG charities scraped: {0}", _charities.Count-1);
            Console.WriteLine("Total distinct charities: {0}", distinctCharityNames.Count()-1);

            var filePathAndName = Environment.CurrentDirectory + "\\charities-vmg.csv";
            File.WriteAllText(filePathAndName, "VMG scraper run on " + DateTime.Now.ToString("dd MMM yyyy") + Environment.NewLine);
            File.AppendAllLines(filePathAndName,distinctCharityNames);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("charities-vmg file saved. Total VMG scraper running duration: " + (DateTime.Now - timeStarted).ToString());
            Console.WriteLine("Press Enter to continue.");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ReadLine();
        }

        public void ScrapeCategoriesListPage()
        {
            var cats = new HtmlWeb {UseCookies = true, PostResponse = SaveCookies};

            var categoriesListPage = cats.Load("http://uk.virginmoneygiving.com/fundraiser-web/donate/charityCategories.action");

            _linksToCategoryPages.AddRange(GetLinksToCategoryPages(categoriesListPage));
        }

        public void ScrapeSubcategoriesListPages()
        {
            var subcats = new HtmlWeb { UseCookies = true, PreRequest = LoadCookies, PostResponse = SaveCookies };
            foreach (var subcategoriesPageUrl in _linksToCategoryPages)
            {
                var subCategoriesListPage = subcats.Load(domain + subcategoriesPageUrl); 
                var linksToCharityListPages = from lnks in subCategoriesListPage.DocumentNode.Descendants()
                                            where lnks.Name == "a" &&
                                            lnks.InnerText.Trim().Length > 0 &&
                                            lnks.Attributes["href"].Value.StartsWith(
                                                "/fundraiser-web/donate/charityCategoryListing.action?categoryId=")
                                            select new
                                            {
                                                Url = lnks.Attributes["href"].Value,
                                                Text = lnks.InnerText
                                            };

                _linksToCharityListPages.AddRange(linksToCharityListPages.Select(x => x.Url));
            }
            
        }

        public void ScrapeCharitiesListPage()
        {
            var request = new HtmlWeb { UseCookies = true, PreRequest = LoadCookies };
            foreach (var charityListPageUrl in _linksToCharityListPages)
            {
                var page = request.Load(domain + charityListPageUrl);
                var nextPageLink = GetNextPageLink(page);

                GetCharityNamesFromCharityListPage(page);

                while (!string.IsNullOrEmpty(nextPageLink))
                {
                    var nextPage = request.Load(domain + "/fundraiser-web/donate/" + nextPageLink);
                    GetCharityNamesFromCharityListPage(nextPage);
                    nextPageLink = HttpUtility.HtmlDecode(GetNextPageLink(nextPage));
                    Console.WriteLine("-----------------");
                }
            }
        }

        private static string GetNextPageLink(HtmlDocument page)
        {
            return page.DocumentNode.Descendants().Where(links => links.Name == "a" &&
                                                                           links.InnerText != null &&
                                                                           links.InnerText.StartsWith("Next"))
                                .Select(links => links.Attributes["href"].Value).FirstOrDefault();
        }

        private static IEnumerable<string> GetLinksToCategoryPages(HtmlDocument categoriesPage)
        {
            var linksToCategoryPages = from lnks in categoriesPage.DocumentNode.Descendants()
                                       where lnks.Name == "a" &&
                                             lnks.InnerText.Trim().Length > 0 &&
                                             lnks.Attributes["href"].Value.StartsWith(
                                                 "/fundraiser-web/donate/charitySubcategories.action")
                                       select new
                                           {
                                               Url = lnks.Attributes["href"].Value,
                                               Text = lnks.InnerText
                                           };
            return linksToCategoryPages.Select(x => x.Url).ToList();
        }

        protected void SaveCookies(HttpWebRequest request, HttpWebResponse response)
        {
            if (response.Cookies.Count > 0)
                _cookieCollection = response.Cookies;
        }

        internal bool LoadCookies(HttpWebRequest request)
        {
            request.CookieContainer.Add(_cookieCollection);
            return true;
        }

        private void GetCharityNamesFromCharityListPage(HtmlDocument charitiesPage)
        {
            var charities = (from lnks in charitiesPage.DocumentNode.Descendants()
                               where lnks.Name == "a" &&
                                       lnks.InnerText.Trim().Length > 0 &&
                                       lnks.Attributes["href"].Value.StartsWith(
                                           "/charity-web/charity/finalCharityHomepage.action?charityId=")
                               select lnks.InnerText + ", " + lnks.ParentNode.NextSibling.NextSibling.ChildNodes[0].InnerText.Replace("\r\n\t\t\t\t\t\t\t\t\t\t","").Replace("Reg No. ",string.Empty)).ToList();

            charities.ForEach(c => Console.WriteLine("Adding charity: " + c));

            _charities.AddRange(charities);
            
            Thread.Sleep(4000); //avoid thresholds/alerts etc
        }
    }
}