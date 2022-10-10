using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bogus;
using Bogus.DataSets;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using PinterestDesktopBot.PinterestClient.Extensions;
using PinterestDesktopBot.PinterestClient.Models;
using Pop3;
using Cookie = System.Net.Cookie;
using Exception = System.Exception;
using Uri = System.Uri;

namespace PinterestDesktopBot.PinterestClient.Api
{
    internal class WebApi : IWebApi
    {
        private PinterestApi Api { get; set; }

        private string _driverPath;
        private readonly string _driverExecutableFileName;
        private readonly ChromeOptions _chromeOptions;
        private string _emailVerifyLink;

        private const string PopServer = "116.203.221.31";

        public WebApi(PinterestApi api)
        {
            Api = api;

            _driverPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _driverExecutableFileName =
                RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "chromedriver" : "chromedriver.exe";

            _chromeOptions = new ChromeOptions();

            _chromeOptions.AddArguments("no-sandbox");
            // _chromeOptions.AddArguments("headless");
            // _chromeOptions.AddArguments("start-maximized");
            _chromeOptions.AddArguments("window-size=1920,1080");
            _chromeOptions.AddArgument($"user-agent={PinterestApiConstants.HeaderUserAgent}");
            _chromeOptions.AddArguments("incognito");
            _chromeOptions.AddExtension(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
                "Extensions/blockimage.crx"));
        }

        public AutoRegisteredAccount CreateAccount(string proxy = null, string mailProvider = null,
            bool emailVerify = false,
            string country = "US", string locale = "en-US")
        {
            var faker = new Faker();

            if (mailProvider != null)
            {
                mailProvider = Guid.NewGuid().ToString("N") + "." + mailProvider;
            }

            var gender = faker.Person.Gender;
            var name = faker.Name.FirstName(gender);
            var surname = faker.Name.LastName(gender);
            var username =
                (name.ToLowerInvariant() + "_" + surname.ToLowerInvariant() + faker.Random.Number(99)).Replace(" ",
                    string.Empty);
            var email = faker.Internet.Email(name.ToLowerInvariant(), surname.ToLowerInvariant(), mailProvider,
                faker.Random.Number(99).ToString());
            var password = faker.Internet.Password(16);
            var businessName = $"{name} {surname}";
            // var avatarUrl = faker.Internet.Avatar();

            if (mailProvider != null)
            {
                var vestaClient =
                    new global::VestaClient.VestaClient($"https://{PopServer}:8083/", "admin", "Ea1021+2015");

                Task.Run(() => vestaClient.Mail.CreateMailDomain(mailProvider)).Wait();
                Task.Run(() => vestaClient.Mail.CreateMailAddress(mailProvider, email.Split('@')[0], password)).Wait();
            }

            if (!string.IsNullOrEmpty(proxy))
            {
                _chromeOptions.Proxy = new Proxy()
                {
                    Kind = ProxyKind.Manual,
                    IsAutoDetect = false,
                    HttpProxy = proxy,
                    SslProxy = proxy
                };
                _chromeOptions.AddArgument("ignore-certificate-errors");
            }

            using var service = ChromeDriverService.CreateDefaultService(_driverPath, _driverExecutableFileName);
            using var driver = new ChromeDriver(service, _chromeOptions, TimeSpan.FromSeconds(60));

            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
                driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(60);

                driver.Navigate().GoToUrl("chrome://extensions/?id=pehaalcefcjfccdpbckoablngfkfgfgj");
                driver.ExecuteScript(
                    "document.querySelector('extensions-manager').shadowRoot.querySelector('#viewManager > extensions-detail-view.active').shadowRoot.querySelector('div#container.page-container > div.page-content > div#options-section extensions-toggle-row#allow-incognito').shadowRoot.querySelector('label#label input').click()");

                Console.WriteLine($"Creating new account: {email}");

                driver.Navigate().GoToUrl(PinterestApiConstants.UrlBase);

                // driver.FindElement(By.XPath("//div[contains(text(),'Bir işletme hesabı oluşturun')]")).Click();
                driver.FindElement(By.CssSelector("div[role='button'][tabindex='0']")).Click();
                driver.FindElement(By.Id("email")).SendKeys(email);
                driver.FindElement(By.Id("password")).SendKeys(password);
                driver.FindElement(By.CssSelector("button[type='submit']")).Click();

                IWebElement businessNameElement;

                try
                {
                    var bwait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                    businessNameElement = bwait.Until(webDriver =>
                    {
                        try
                        {
                            return webDriver.FindElement(By.Id("businessName"));
                        }
                        catch (NoSuchElementException e)
                        {
                            return null;
                        }
                    });
                }
                catch
                {
                    businessNameElement = null;
                }

                if (businessNameElement != null)
                {
                    businessNameElement.SendKeys(businessName);
                    driver.FindElements(By.CssSelector("label[for='websiteNotYet']")).LastOrDefault()?.Click();
                    driver.FindElements(By.Id("newUserCountry")).LastOrDefault()?.Click();
                    driver.FindElement(By.CssSelector($"#newUserCountry option[value='{country}']")).Click();
                    driver.FindElement(By.Id("newUserLanguage")).Click();
                    driver.FindElement(By.CssSelector($"#newUserLanguage option[value='{locale}']")).Click();

                    driver.FindElements(By.CssSelector("div[role='dialog'] button[type='button']")).LastOrDefault()
                        ?.Click();

                    Task.Delay(TimeSpan.FromSeconds(15)).Wait();

                    try
                    {
                        var businessNameElement2 = new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                            .Until(webDriver =>
                            {
                                try
                                {
                                    return webDriver.FindElement(By.Id("businessName"));
                                }
                                catch (NoSuchElementException e)
                                {
                                    return null;
                                }
                            });

                        businessNameElement2.Clear();
                        businessNameElement2.SendKeys(businessName);
                        driver.FindElements(By.CssSelector("label[for='websiteNotYet']")).LastOrDefault()?.Click();
                        driver.FindElements(By.Id("newUserCountry")).LastOrDefault()?.Click();
                        driver.FindElement(By.CssSelector($"#newUserCountry option[value='{country}']")).Click();
                        driver.FindElement(By.Id("newUserLanguage")).Click();
                        driver.FindElement(By.CssSelector($"#newUserLanguage option[value='{locale}']")).Click();

                        driver.FindElements(By.CssSelector("div[role='dialog'] button[type='button']")).LastOrDefault()
                            ?.Click();
                    }
                    catch
                    {
                        // ignored
                    }

                    try
                    {
                        driver.FindElement(By.CssSelector("label[for='blogger']")).Click();
                        driver.FindElements(By.CssSelector("div[role='dialog'] button[type='button']")).LastOrDefault()
                            ?.Click();
                    }
                    catch
                    {
                        //ignored
                    }

                    driver.FindElement(By.CssSelector("label[for='advertisingIntentYes']")).Click();
                    driver.FindElements(By.CssSelector("div[role='dialog'] button[type='button']")).LastOrDefault()
                        ?.Click();
                }
                else
                {
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                    var newUserLanguage = wait.Until(webDriver =>
                    {
                        try
                        {
                            return webDriver.FindElement(By.Id("newUserLanguage"));
                        }
                        catch (NoSuchElementException e)
                        {
                            return null;
                        }
                    });

                    newUserLanguage.Click();
                    driver.FindElement(By.CssSelector($"#newUserLanguage option[value='{locale}']")).Click();
                    driver.FindElement(By.Id("newUserCountry")).Click();
                    driver.FindElement(By.CssSelector($"#newUserCountry option[value='{country}']")).Click();
                    driver.FindElement(By.CssSelector(".NuxContainer__NuxStepContainer button[type='submit']")).Click();

                    Task.Delay(TimeSpan.FromSeconds(15)).Wait();

                    try
                    {
                        new WebDriverWait(driver, TimeSpan.FromSeconds(30))
                            .Until(webDriver =>
                            {
                                try
                                {
                                    return webDriver.FindElement(By.Id("newUserLanguage"));
                                }
                                catch (NoSuchElementException e)
                                {
                                    return null;
                                }
                            });

                        driver.FindElement(By.CssSelector(".NuxContainer__NuxStepContainer button[type='submit']"))
                            .Click();
                    }
                    catch
                    {
                        // ignored
                    }

                    driver.FindElement(By.Id("name")).SendKeys(businessName);
                    driver.FindElement(By.CssSelector(".NuxContainer__NuxStepContainer button[type='button']")).Click();

                    // for (int i = 0; i < 3; i++)
                    // {
                    try
                    {
                        driver.FindElement(
                            By.CssSelector(".NuxContainer__NuxStepContainer button[type='submit']")).Click();
                        driver.FindElement(
                            By.CssSelector(".NuxContainer__NuxStepContainer button[type='button']")).Click();
                    }
                    catch
                    {
                        // ignored
                    }
                    // }

                    driver.FindElement(By.CssSelector("label[for='adv_intentYes']")).Click();
                    driver.FindElement(By.CssSelector(".NuxContainer__NuxStepContainer button[type='button']")).Click();

                    try
                    {
                        driver.FindElement(By.CssSelector("label[for='contact_infoNo']")).Click();
                        driver.FindElement(
                                By.CssSelector(".NuxContainer__NuxStepContainer button[type='button']:not(:disabled)"))
                            .Click();
                    }
                    catch
                    {
                        //ignored
                    }

                    // try
                    // {
                    //     var fwait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                    //
                    //     var interestEls = fwait.Until(webDriver =>
                    //     {
                    //         try
                    //         {
                    //             return webDriver
                    //                 .FindElements(By.CssSelector("div[data-test-id='nux-picker-topic']"))
                    //                 .Take(9)
                    //                 .OrderBy(o => Guid.NewGuid())
                    //                 .Take(5)
                    //                 .ToList();
                    //         }
                    //         catch (NoSuchElementException e)
                    //         {
                    //             return null;
                    //         }
                    //     });
                    //
                    //     foreach (var interestEl in interestEls)
                    //     {
                    //         interestEl.Click();
                    //     }
                    //
                    //     driver.FindElement(By.CssSelector("button[type='button']")).Click();
                    // }
                    // catch
                    // {
                    //     // ignored
                    // }

                    // try
                    // {
                    //     driver.FindElement(By.CssSelector(".BizNuxExtensionUpsell__optionalSkip")).Click();
                    // }
                    // catch
                    // {
                    //     // ignored
                    // }
                }

                driver.Navigate().GoToUrl(PinterestApiConstants.UrlBase + "settings/");
                driver.FindElement(By.Id("username")).Clear();
                driver.FindElement(By.Id("username")).SendKeys(username);
                driver.FindElement(By.Id("about")).SendKeys(faker.Name.JobTitle());
                driver.FindElement(By.CssSelector("div[data-test-id='done-button'] button[type='button']")).Click();

                if (emailVerify)
                {
                    try
                    {
                        driver.FindElement(By.CssSelector("a[href='/settings/security']")).Click();
                        driver.FindElement(By.CssSelector(".mainContainer a[href='#']")).Click();
                        driver.FindElement(By.CssSelector("div[aria-label] button[type='button']")).Click();
                    }
                    catch
                    {
                        // ignored
                    }

                    driver.Navigate().GoToUrl("data:,");

                    Console.WriteLine($"{DateTime.Now:G} -> Doğrulama maili bekleniyor: {email}");

                    Task.Delay(TimeSpan.FromSeconds(30)).Wait();

                    try
                    {
                        WaitWhile(async () =>
                        {
                            using var pop3Client = new Pop3Client();

                            await pop3Client.ConnectAsync(PopServer, email, password, false);

                            var confirmMail =
                                (await pop3Client.ListAndRetrieveHeaderAsync()).FirstOrDefault(message =>
                                    message.From.Contains("confirm@account.pinterest.com"));

                            if (confirmMail == null)
                            {
                                return true;
                            }

                            await pop3Client.RetrieveAsync(confirmMail);

                            if (!confirmMail.Retrieved)
                            {
                                return true;
                            }

                            var link = Regex.Matches(confirmMail.Body,
                                    "(https://post.pinterest.com/(.*)(autologin)(.*))$", RegexOptions.Multiline)
                                .Select(m => m.Groups[0].Value).FirstOrDefault(s => s.Contains("autologin"));

                            if (link == null)
                            {
                                return true;
                            }

                            _emailVerifyLink = link;

                            return false;
                        }, 60000, 300000);
                    }
                    catch
                    {
                        Console.WriteLine($"{DateTime.Now:G} -> Doğrulama Maili Gelmedi: {email}");
                        throw;
                    }

                    if (!string.IsNullOrEmpty(_emailVerifyLink))
                    {
                        driver.Navigate().GoToUrl(_emailVerifyLink);
                        Console.WriteLine($"{DateTime.Now:G} -> Email doğrulandı: {email}");
                    }
                    else
                    {
                        Console.WriteLine($"{DateTime.Now:G} -> Email doğrulanamadı: {email}");
                    }
                }

                // var httpClient = new PinterestClient(string.Empty, string.Empty);
                //
                // var userInfo = httpClient.Pinners.GetUserInfoAsync(username).Result;
                //
                // if (userInfo == null || userInfo.Indexed != true)
                // {
                //     throw new Exception("Hesap bulunamadı veya shadowlu.");
                // }

                var account = new AutoRegisteredAccount
                {
                    Gender = gender == Name.Gender.Male ? "male" : "female",
                    Name = name,
                    Surname = surname,
                    UserName = username,
                    Email = email,
                    Password = password,
                    BusinessName = businessName,
                    Cookies = driver.Manage().Cookies.AllCookies.Select(c => new Cookie
                    {
                        Domain = c.Domain,
                        Expires = c.Expiry ?? DateTime.MinValue,
                        HttpOnly = c.IsHttpOnly,
                        Name = c.Name,
                        Path = c.Path,
                        Secure = c.Secure,
                        Value = c.Value,
                    }).ToList(),
                };

                driver.Manage().Cookies.DeleteAllCookies();

                return account;
            }
            finally
            {
                driver.Manage().Cookies.DeleteAllCookies();
                driver.Quit();
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.Unicode.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private string ReadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var resourcePath = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(name));

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static void WaitWhile(Func<Task<bool>> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (await condition()) await Task.Delay(frequency);
            });

            if (waitTask != Task.Run(() => Task.WhenAny(waitTask, Task.Delay(timeout))).Result)
                throw new TimeoutException();
        }
    }
}