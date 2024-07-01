using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotNetEnv;
using Microsoft.Playwright;

namespace PlaywrightDemo
{
    class Example
    {
        static async Task Main(string[] args)
        {
            Env.Load();

            // Retrieve proxy settings from environment variables.
            string proxyServer = Env.GetString("PROXY_SERVER");
            string proxyUsername = Env.GetString("PROXY_USERNAME");
            string proxyPassword = Env.GetString("PROXY_PASSWORD");
            string siteUsername = Env.GetString("LINKED_USERNAME");
            string sitePassword = Env.GetString("LINKED_PASSWORD");

            try
            {
                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(
                    new BrowserTypeLaunchOptions { Headless = true }
                );

                var contextOptions = new BrowserNewContextOptions()
                {
                    Proxy = new Proxy()
                    {
                        Server = proxyServer,
                        Username = proxyUsername,
                        Password = proxyPassword,
                    }
                };

                var context = await browser.NewContextAsync(contextOptions);
                var page = await context.NewPageAsync();

                await page.GotoAsync("https://www.linkedin.com/home");
                // await page.GotoAsync("https://bestbuy.ca/");

                // var signInButton = page.GetByRole(
                //     AriaRole.Link,
                //     new() { Name = "Account", Exact = true }
                // );
                // if (signInButton == null)
                //     throw new NullReferenceException("Sign-in button not found.");

                // await signInButton.ClickAsync();

                // Wait for load state.
                await page.WaitForLoadStateAsync(LoadState.Load);

                // Fill email and password fields.
                var emailField = page.GetByLabel("Email Address");
                if (emailField == null)
                    throw new NullReferenceException("Email field not found.");

                await emailField.FillAsync(siteUsername);

                var passwordField = page.GetByLabel("Password");
                if (passwordField == null)
                    throw new NullReferenceException("Password field not found.");

                await passwordField.FillAsync(sitePassword);

                var signInButton = page.GetByRole(AriaRole.Button, new() { Name = "Sign in" });
                if (await signInButton.CountAsync() == 0)
                    throw new NullReferenceException("Sign-in button not found.");

                await signInButton.ClickAsync();

                await page.WaitForURLAsync("https://www.linkedin.com/in/jaykchen/");

                var pageText = await page.TextContentAsync("body");

                Console.WriteLine(pageText);

/*                 try
                {
                    await page.GotoAsync("https://www.linkedin.com/in/jaykchen/");

                    // Locate sign-in button and error message locator.
                    var signInBut2 = page.Locator(
                        "[data-automation='registered-sign-in'] .signin-form-button"
                    );
                    if (signInBut2 == null)
                        throw new NullReferenceException("Second Sign-in button not found.");

                    var errorMessageLocator = page.Locator(
                        "[data-automation='form-warning-message'] p"
                    );

                    // Wait for the sign-in button to be visible and click it.
                    Console.WriteLine("Waiting for the sign-in button...");
                    await signInBut2.WaitForAsync(
                        new LocatorWaitForOptions { State = WaitForSelectorState.Visible }
                    );

                    Console.WriteLine("Clicking the sign-in button...");
                    await signInBut2.ClickAsync();

                    // Wait for network idle state after clicking login.
                    Console.WriteLine("Waiting for network idle state...");
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                    // Take screenshot post-login attempt.
                    Console.WriteLine("Taking a screenshot post-login attempt...");
                    await page.ScreenshotAsync(new() { Path = "login_screenshot.png" });

                    bool hasErrorMessageElement = (await errorMessageLocator.CountAsync()) > 0;
                    Console.WriteLine($"Error message element exists:{hasErrorMessageElement}");

                    if (await errorMessageLocator.IsVisibleAsync())
                    {
                        string errormessage = (await errorMessageLocator.TextContentAsync());
                        Console.WriteLine($"Login failed with message:{errormessage}");
                    }
                    else
                    {
                        Console.WriteLine("No visible error message found.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Inner exception occurred :{ex.Message}");
                } */
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Outer exception occurred :{ex.Message}");
            }
        }
    }
}
