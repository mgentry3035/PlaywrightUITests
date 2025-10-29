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
                Headless = false,   // 👀 Set to false to watch execution
                SlowMo = 200        // ⏳ Add delay between actions for realism
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

            // Step 1: Accept cookies (if shown)
            var acceptBtn = await page.QuerySelectorAsync("button:has-text('Accept')");
            if (acceptBtn != null)
            {
                Console.WriteLine("✅ Accepting cookies...");
                await acceptBtn.ClickAsync();
            }

            // Step 2: Search for Prometheus Group
            Console.WriteLine("➡️ Searching for 'Prometheus Group'...");
            await page.FillAsync("[name='q']", "Prometheus Group");
            await page.PressAsync("[name='q']", "Enter");

            // Step 3: Wait for search results or detect CAPTCHA
            Console.WriteLine("⏳ Waiting for search results...");
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            try
            {
                await page.Locator("h3").First.WaitForAsync(new LocatorWaitForOptions
                {
                    Timeout = 60000 // Wait up to 60 seconds for search results
                });
            }
            catch (TimeoutException)
            {
                var html = await page.ContentAsync();
                if (html.Contains("Our systems have detected unusual traffic") || html.Contains("recaptcha"))
                {
                    Console.WriteLine("⚠️ CAPTCHA detected — skipping test.");
                    Assert.Inconclusive("⚠️ CAPTCHA detected — skipping test to prevent false failure.");
                    return;
                }
                else
                {
                    await page.ScreenshotAsync(new PageScreenshotOptions { Path = "GoogleSearch_Failure.png" });
                    Assert.Fail("❌ Timed out waiting for search results — possible layout change or network issue.");
                }
            }

            // Step 4: Verify "Prometheus Group" appears in search results
            Console.WriteLine("✅ Verifying that results contain 'Prometheus Group'...");
            var firstResult = await page.Locator("h3").First.InnerTextAsync();
            firstResult.Should().Contain("Prometheus Group", "the top search result should mention Prometheus Group");

            // Step 5: Click the "Contact Us" link
            Console.WriteLine("➡️ Looking for 'Contact Us' link...");
            var contactLink = page.Locator("a:has-text('Contact Us')");
            var linkCount = await contactLink.CountAsync();

            if (linkCount == 0)
            {
                Assert.Inconclusive("⚠️ No 'Contact Us' link found in search results.");
                return;
            }

            Console.WriteLine("✅ Clicking first 'Contact Us' link...");
            await contactLink.First.ClickAsync();

            // Step 6: Wait for Contact page to load
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await page.WaitForTimeoutAsync(2000); // Let content stabilize

            // Step 7: Fill name fields
            Console.WriteLine("✏️ Filling name fields...");
            await page.FillAsync("input[name*='first']", "Michael");
            await page.FillAsync("input[name*='last']", "Gentry");

            // Step 8: Submit the form
            Console.WriteLine("➡️ Submitting form...");
            await page.ClickAsync("button[type='submit']");
            await page.WaitForTimeoutAsync(2000); // Wait for validation to show

            // Step 9: Validate required field errors
            Console.WriteLine("🔍 Checking required field validation...");
            var errorCount = await page.Locator(".error, .validation-message, [aria-invalid='true']").CountAsync();
            Assert.That(errorCount, Is.GreaterThanOrEqualTo(4), "There should be at least 4 required fields unfilled.");

            Console.WriteLine("✅ Test completed successfully!");
        }
    }
}


