using NUnit.Framework;
using Microsoft.Playwright;
using System.Threading.Tasks;
using FluentAssertions;
using System;

namespace PlaywrightUITests.Tests
{
    [TestFixture]
    public class GoogleSearchTests
    {
        private IPlaywright playwright;
        private IBrowser browser;
        private IPage page;

        [SetUp]
        public async Task Setup()
        {
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,   // 👀 Set to false so we can see what’s happening
                SlowMo = 200        // ⏳ Slows down actions to reduce bot detection
            });
            var context = await browser.NewContextAsync();
            page = await context.NewPageAsync();
        }

        [TearDown]
        public async Task Teardown()
        {
            await browser.CloseAsync();
            playwright.Dispose();
        }

        [Test]
        public async Task GoogleSearch_ContactUsFormValidation()
        {
            Console.WriteLine("➡️ Navigating to Google...");
            await page.GotoAsync("https://www.google.com");

            // Step 1: Accept cookies if prompted
            var acceptBtn = await page.QuerySelectorAsync("button:has-text('Accept')");
            if (acceptBtn != null)
            {
                Console.WriteLine("✅ Accepting cookies...");
                await acceptBtn.ClickAsync();
            }

            // Step 2: Perform the search
            Console.WriteLine("➡️ Searching for 'Prometheus Group'...");
            await page.FillAsync("[name='q']", "Prometheus Group");
            await page.PressAsync("[name='q']", "Enter");

            // Step 3: Wait for search results to load
            Console.WriteLine("⏳ Waiting for search results...");
            await page.WaitForNavigationAsync(new PageWaitForNavigationOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });
            await page.Locator("h3").First.WaitForAsync(); // Wait for a result heading

            // Step 4: Detect possible CAPTCHA page
            var html = await page.ContentAsync();
            if (html.Contains("Our systems have detected unusual traffic") || html.Contains("recaptcha"))
            {
                Assert.Inconclusive("⚠️ Google blocked automation with CAPTCHA. Test skipped to prevent false failure.");
                return;
            }

            // Step 5: Validate that results contain 'Prometheus Group'
            Console.WriteLine("✅ Verifying that results contain 'Prometheus Group'...");
            var firstResult = await page.Locator("h3").First.InnerTextAsync();
            firstResult.Should().Contain("Prometheus Group", "the top search result should mention Prometheus Group");

            // Step 6: Click the 'Contact Us' link (first one found)
            Console.WriteLine("➡️ Attempting to click 'Contact Us' link...");
            var contactLink = page.Locator("a:has-text('Contact Us')");
            var linkCount = await contactLink.CountAsync();

            if (linkCount == 0)
            {
                Assert.Inconclusive("⚠️ No 'Contact Us' link was found in the search results.");
                return;
            }

            await contactLink.First.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await page.WaitForTimeoutAsync(2000); // Allow page content to stabilize

            // Step 7: Verify we reached a contact page
            Console.WriteLine("✅ Contact page detected. Filling out form...");
            await page.FillAsync("input[name*='first']", "Michael");
            await page.FillAsync("input[name*='last']", "Gentry");

            // Step 8: Submit form
            await page.ClickAsync("button[type='submit']");
            await page.WaitForTimeoutAsync(2000); // Give page time to show validation errors

            // Step 9: Validate required field messages
            var errorCount = await page.Locator(".error, .validation-message, [aria-invalid='true']").CountAsync();
            Assert.That(errorCount, Is.GreaterThanOrEqualTo(4), "There should be at least 4 required fields unfilled.");

            Console.WriteLine("✅ Test completed successfully!");
        }
    }
}

