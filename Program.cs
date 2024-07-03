using System;
using System.IO;
using System.Text.Json;
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
                    new BrowserTypeLaunchOptions { Headless = true }
                );

                var contextOptions = new BrowserNewContextOptions()
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:31.0) Gecko/20100101 Firefox/31.0",
                    IgnoreHTTPSErrors = true
                };

                var context = await browser.NewContextAsync(contextOptions);
                var page = await context.NewPageAsync();

                // Load cookies from file and set them in the context.
                if (File.Exists("cookies.json"))
                {
                    var cookieJson = await File.ReadAllTextAsync("cookies.json");
                    var cookiesArray = JsonSerializer.Deserialize<Cookie[]>(cookieJson);

                    if (cookiesArray != null)
                    {
                        // foreach (var cookie in cookiesArray)
                        // {
                        //     // Adjust domain if necessary.
                        //     cookie.Domain ??= ".linkedin.com";
                        // }

                        await context.AddCookiesAsync(cookiesArray);
                        Console.WriteLine("Cookies loaded successfully.");
                    }
                    else
                    {
                        Console.WriteLine("No valid cookies found.");
                    }
                }
                else
                {
                    Console.WriteLine("No cookie file found.");
                }

                await page.GotoAsync(
                    "https://www.linkedin.com/home",
                    new PageGotoOptions { Timeout = (int)TimeSpan.FromMinutes(2).TotalMilliseconds }
                );

                await page.WaitForLoadStateAsync(LoadState.Load);

                Console.WriteLine("Reached after login prompt");

                // if (page.Url.Contains("feed"))
                // {
                //     Console.WriteLine("URL contains 'feed'");
                // }
                // else
                // {
                //     Console.WriteLine("URL does not contain 'feed'");
                // }

                await page.ScreenshotAsync(new() { Path = "profile_screenshot.png" });

                // var cookies = await context.CookiesAsync();
                // await System.IO.File.WriteAllTextAsync(
                //     "cookies.json",
                //     System.Text.Json.JsonSerializer.Serialize(cookies)
                // );
                // Console.WriteLine(cookies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Outer exception occurred :{ex.Message}");
            }
        }
    }
}
