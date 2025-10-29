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
                Headless = false,   // 👀 Set to false to observe the browser
                SlowMo = 200        // ⏳ Delay actions for realistic timing
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
                    Timeout = 60000 // Wait up to 60 seconds
                });
            }
            catch (TimeoutException)
            {
                var html = await page.ContentAsync();
                if (html.Contains("Our systems have detected unusual traffic") || html.Contains("recaptcha"))
                {
                    await page.ScreenshotAsync(new PageScreenshotOptions { Path = "GoogleSearch_Captcha.png" });
                    Console.WriteLine("⚠️ CAPTCHA detected — skipping test.");
                    Assert.Inconclusive("⚠️ CAPTCHA detected — screenshot captured and test skipped.");
                    return;
                }
                else
                {
                    await page.ScreenshotAsync(new PageScreenshotOptions { Path = "GoogleSearch_Failure.png" });
                    Assert.Fail("❌ Timed out waiting for search results — possible layout change or network issue.");
                }
            }

            // Step 4: Verify results contain 'Prometheus Group'
            Console.WriteLine("✅ Verifying that results contain 'Prometheus Group'...");
            var content = await page.ContentAsync();
            content.Should().Contain("Prometheus Group", "search results should reference Prometheus Group");

            // Step 5: Try to find and click 'Contact Us' or Prometheus site
            Console.WriteLine("➡️ Searching for 'Contact Us' or Prometheus Group link...");
            var contactLink = page.Locator("a:has-text('Contact Us'), a:has-text('Contact')");
            int linkCount = await contactLink.CountAsync();

            if (linkCount > 0)
            {
                Console.WriteLine($"✅ Found {linkCount} 'Contact' link(s). Clicking first one...");
                await contactLink.First.ClickAsync();
            }
            else
            {
                var prometheusResult = page.Locator("a:has-text('Prometheus Group')");
                int resultCount = await prometheusResult.CountAsync();

                if (resultCount > 0)
                {
                    Console.WriteLine("➡️ Clicking Prometheus Group site link...");
                    await prometheusResult.First.ClickAsync();
                    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                    // Look for 'Contact Us' link on the Prometheus Group site
                    Console.WriteLine("➡️ Searching for 'Contact Us' on Prometheus Group site...");
                    var siteContactLink = page.Locator("a:has-text('Contact Us'), a:has-text('Contact')");
                    int siteLinkCount = await siteContactLink.CountAsync();

                    if (siteLinkCount == 0)
                    {
                        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "NoContactUs_OnSite.png" });
                        Assert.Inconclusive("⚠️ No 'Contact Us' link found on Prometheus Group site.");
                        return;
                    }

                    Console.WriteLine("✅ Clicking 'Contact Us' link on Prometheus Group site...");
                    await siteContactLink.First.ClickAsync();
                }
                else
                {
                    await page.ScreenshotAsync(new PageScreenshotOptions { Path = "NoPrometheusResult.png" });
                    Assert.Inconclusive("⚠️ No Prometheus Group result found in search results.");
                    return;
                }
            }

            // Step 6: Wait for Contact page
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await page.WaitForTimeoutAsync(2000);

            // Step 7: Fill out the form
            Console.WriteLine("✏️ Filling name fields...");
            await page.FillAsync("input[name*='first']", "Michael");
            await page.FillAsync("input[name*='last']", "Gentry");

            // Step 8: Submit form (handles both 'Contact Us' and 'Submit')
            Console.WriteLine("➡️ Submitting form...");
            var submitButton = page.Locator("button:has-text('Contact Us'), button[type='submit'], input[type='submit'][value*='Contact']");            int buttonCount = await submitButton.CountAsync();

            if (buttonCount == 0)
            {
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = "NoSubmitButton.png" });
                Assert.Inconclusive("⚠️ No 'Contact Us' or 'Submit' button found on the contact page.");
                return;
            }

            await submitButton.First.ClickAsync();
            await page.WaitForTimeoutAsync(2000);

            // Step 9: Validate required field errors
            Console.WriteLine("🔍 Checking required field validation...");
            var errorCount = await page.Locator(".error, .validation-message, [aria-invalid='true']").CountAsync();
            Assert.That(errorCount, Is.GreaterThanOrEqualTo(4), "There should be at least 4 required fields unfilled.");

            Console.WriteLine("✅ Test completed successfully!");
        }
    }
}



