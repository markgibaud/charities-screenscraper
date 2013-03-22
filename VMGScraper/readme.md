##VMGScraper

**VMGScraper** is quick and dirty utility written to scrape the [VirginMoneyGiving](http://uk.virginmoneygiving.com/giving/) website for all charities listed there.

The utility queries successive pages on the website at 4 second intervals, de-dupes and saves the charity name and registration number to a file "charities.csv" on disk.

This utility will probably be refactored in the near future to include more sites like MyDonate etc.