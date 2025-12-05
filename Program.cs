using Ardalis.GuardClauses;
using Microsoft.Playwright;
using NUnit.Framework;
//using PlaywrightFramework;

using System;
using System.Threading.Tasks;


namespace BBCSportTests
{
    public class TestChallengeTwo 
    {
         private IBrowser browser;
        private IPage page;
        private IPlaywright playwright;
        [SetUp]
        public async Task Setup()
        {
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync();
            page = await context.NewPageAsync();
        }

        [Test]
        public async Task VerifyLasVegasGPResultsAreReported()
        {
            //lauch page
            //await page.PauseAsync();
            await page.GotoAsync("https://www.bbc.com/sport");
           
            //select Formula 1 tab
            var searchLocator = page.GetByTestId("navigation").GetByRole(AriaRole.Link, new() { Name = "Formula" });
            await searchLocator.ClickAsync();

            //select results tab
            var searchBar = page.GetByRole(AriaRole.Link, new() { Name = "Results" });
            await searchBar.ClickAsync();

            //search relevant year
            var searchInput = page.GetByTestId("datepicker-date-link-2023");
            await searchInput.ClickAsync();
           
            // select Las vegas location
            var searchPageTab = page.GetByRole(AriaRole.Button, new() { Name = "Las Vegas Grand Prix, Las Vegas Street Circuit" });
            await searchPageTab.ClickAsync();

            
            //Create dictionary
            var expectedResults = new Dictionary<string, string>
            {
                { "1", "Max Verstappen" },
                { "2", "George Russell" },
                { "3", "Sergio Perez" }
            };
            //Create Dictionary to store actual results
            var actualResults = new Dictionary<string, string>();

            // Locate all rows in the table selector 
            var rows = await page.QuerySelectorAllAsync("tbody tr");
            foreach (var row in rows)
            {
                // Extract Name and Position from columns (adjust selectors based on your table structure)
                var position = await row.QuerySelectorAsync("td:nth-child(1)");
                var name = await row.QuerySelectorAsync("td:nth-child(2)");
                string nameText = await name.InnerTextAsync();
                string positionText = await position.InnerTextAsync();
                actualResults[nameText] = positionText;
            }
            // Compare actual vs expected
            foreach (var expected in expectedResults)
            {
                if (!actualResults.ContainsKey(expected.Key))
                {
                    Console.WriteLine($"Missing Name: {expected.Key}");
                }
                else if (actualResults[expected.Key] != expected.Value)
                {
                    Console.WriteLine($"Mismatch for {expected.Key}: Expected '{expected.Value}', Found '{actualResults[expected.Key]}'");
                }
            }
            // Check for extra names not in expected
            foreach (var actual in actualResults)
            {
                if (!expectedResults.ContainsKey(actual.Key))
                {
                    Console.WriteLine($"Unexpected Name: {actual.Key} with Position '{actual.Value}'");
                }
            }
        }

    }
    

    
}
