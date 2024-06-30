using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightDemo
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions { Headless = true }
            );

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();
            await page.GotoAsync("https://bestbuy.ca/");

            var signInButton = page.GetByRole(
                AriaRole.Link,
                new() { Name = "Account", Exact = true }
            );
            await signInButton.ClickAsync();

            await page.WaitForLoadStateAsync(LoadState.Load);

            await page.GetByLabel("Email Address").FillAsync("jaykchen@gmail.com");


            try
            {
                var signInBut2 = page.Locator("[data-automation='registered-sign-in'] .signin-form-button");
                var errorMessageLocator = page.Locator("[data-automation='form-warning-message'] p");

                // Wait for the sign-in button to be visible and click it
                Console.WriteLine("Waiting for the sign-in button...");
                await signInBut2.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

                Console.WriteLine("Clicking the sign-in button...");
                await signInBut2.ClickAsync();

                // Wait for network idle state
                Console.WriteLine("Waiting for network idle state...");
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                // Take a screenshot after attempting login
                Console.WriteLine("Taking a screenshot post-login attempt...");
                await page.ScreenshotAsync(new() { Path = "login_screenshot.png" });


                bool hasErrorMessageElement = await errorMessageLocator.CountAsync() > 0;
                Console.WriteLine($"Error message element exists:{hasErrorMessageElement}");

                if (await errorMessageLocator.IsVisibleAsync())
                {
                    string errormessage = await errorMessageLocator.TextContentAsync();
                    Console.WriteLine($"Login failed with message:{errormessage}");
                }
                else
                {
                    Console.WriteLine("No visible error message found.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred :{ex.Message}");
            }

        }
    }
}
