using System;
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

            string siteUsername = Env.GetString("LINKED_USERNAME");
            string sitePassword = Env.GetString("LINKED_PASSWORD");

            try
            {
                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Firefox.LaunchAsync(
                    new BrowserTypeLaunchOptions { Headless = false }
                );

                var contextOptions = new BrowserNewContextOptions()
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:31.0) Gecko/20100101 Firefox/31.0",
                    IgnoreHTTPSErrors = true
                };

                var context = await browser.NewContextAsync(contextOptions);
                var page = await context.NewPageAsync();

                await page.GotoAsync(
                    "https://www.linkedin.com/home",
                    new PageGotoOptions { Timeout = (int)TimeSpan.FromMinutes(2).TotalMilliseconds }
                );

                await page.WaitForLoadStateAsync(LoadState.Load);

                // Console.WriteLine("passed home");

                // Console.WriteLine("Please login manually and then press Enter to continue...");
                Console.ReadLine();
                // await page.WaitForTimeoutAsync(30000);
                // Console.WriteLine("Reached after login prompt");
                await page.PauseAsync();

                // if (page.Url.Contains("feed"))
                // {
                //     Console.WriteLine("URL contains 'feed'");
                // }
                // else
                // {
                //     Console.WriteLine("URL does not contain 'feed'");
                // }

                // try
                // {
                //     await page.GotoAsync(
                //         "https://www.linkedin.com/in/jaykchen/",
                //         new PageGotoOptions { Timeout = 60000 }
                //     );
                //     await page.WaitForLoadStateAsync(LoadState.Load);
                //     Console.WriteLine("Navigated to profile page");
                // }
                // catch (Exception ex)
                // {
                //     Console.WriteLine($"Navigation to profile page failed: {ex.Message}");
                // }

                await page.ScreenshotAsync(new() { Path = "profile_screenshot.png" });

                var cookies = await context.CookiesAsync();
                await System.IO.File.WriteAllTextAsync(
                    "cookies.json",
                    System.Text.Json.JsonSerializer.Serialize(cookies)
                );
                Console.WriteLine(cookies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Outer exception occurred :{ex.Message}");
            }
        }
    }
}
