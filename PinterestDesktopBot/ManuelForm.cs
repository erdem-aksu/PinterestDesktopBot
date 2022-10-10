using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bogus;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.Remote;
using PinterestDesktopBot.PinterestClient.Api;
using PinterestDesktopBot.PinterestClient.Extensions;
using Pop3;
using RestSharp;
using Cookie = System.Net.Cookie;

namespace PinterestDesktopBot
{
    public partial class ManuelForm : Form
    {
        private const string UsersFileName = "accounts.txt";
        private const string ApisFileName = "apis.txt";
        private const string LogsFileName = "logs.txt";
        private const string IpsFileName = "ips.txt";
        private const string MailDomain = "sanrecu.com";
        private const string PopServer = "116.203.221.31";

        private readonly PinterestClient.PinterestClient _pinterestClient;

        private string _email;
        private string _password;
        private string _emailVerifyLink;
        private string _username;
        private RemoteWebDriver _driver;
        private DriverService _service;

        public ManuelForm()
        {
            _pinterestClient = new PinterestClient.PinterestClient(string.Empty, string.Empty);
            InitializeComponent();

            Console.SetOut(new RichTextBoxWriter(richTextBox1));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            Console.WriteLine($"{DateTime.Now:G} -> Hizmet başlatıldı.");

            var thread = new Thread(() =>
            {
                var faker = new Faker();

                var mailProvider = MailDomain;

                if (mailProvider != null)
                {
                    mailProvider = faker.Random.Word().ToSlug(true, 15).ToLowerInvariant() + "." + mailProvider;
                }

                var gender = faker.Person.Gender;
                var name = faker.Name.FirstName(gender);
                var surname = faker.Name.LastName(gender);
                _username =
                    (name.ToLowerInvariant() + "_" + surname.ToLowerInvariant() + faker.Random.Number(99)).Replace(
                        " ",
                        string.Empty);
                _email = faker.Internet.Email(name.ToLowerInvariant(), surname.ToLowerInvariant(), mailProvider,
                    faker.Random.Number(99).ToString());
                _password = faker.Internet.Password(16);
                var businessName = $"{name} {surname}";
                // var avatarUrl = faker.Internet.Avatar();

                if (mailProvider != null)
                {
                    var vestaClient =
                        new global::VestaClient.VestaClient($"https://{PopServer}:8083/", "admin", "Ea1021+2015");

                    Task.Run(() => vestaClient.Mail.CreateMailDomain(mailProvider)).Wait();
                    Task.Run(() => vestaClient.Mail.CreateMailAddress(mailProvider, _email.Split('@')[0], _password))
                        .Wait();
                }

                var driverPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var driverExecutableFileName =
                    RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "chromedriver" : "chromedriver.exe";

                if (useOpera.Checked)
                {
                    driverExecutableFileName =
                        RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "operadriver" : "operadriver.exe";

                    var operaOptions = new OperaOptions();

                    operaOptions.AddArguments("no-sandbox");
                    operaOptions.AddArguments("start-maximized");
                    operaOptions.AddArgument($"user-agent={PinterestApiConstants.HeaderUserAgentOpera}");
                    operaOptions.AddArguments("incognito");

                    _service = OperaDriverService.CreateDefaultService(driverPath, driverExecutableFileName);
                    _driver = new OperaDriver((OperaDriverService) _service, operaOptions, TimeSpan.FromSeconds(60));
                }
                else
                {
                    var chromeOptions = new ChromeOptions();

                    chromeOptions.AddArguments("no-sandbox");
                    // _chromeOptions.AddArguments("headless");
                    chromeOptions.AddArguments("start-maximized");
                    // chromeOptions.AddArguments("window-size=1920,1080");
                    chromeOptions.AddArgument($"user-agent={PinterestApiConstants.HeaderUserAgent}");
                    chromeOptions.AddArguments("incognito");
                    chromeOptions.AddExtension(Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location),
                        "Extensions/blockimage.crx"));

                    _service = ChromeDriverService.CreateDefaultService(driverPath, driverExecutableFileName);
                    _driver = new ChromeDriver((ChromeDriverService) _service, chromeOptions, TimeSpan.FromSeconds(60));
                }

                Console.WriteLine(PinterestApiConstants.UrlBase);
                Console.WriteLine(_email);
                Console.WriteLine(_password);
                Console.WriteLine(_username);
                Console.WriteLine(businessName);

                try
                {
                    _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
                    _driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(30);

                    if (!useOpera.Checked)
                    {
                        _driver.Navigate().GoToUrl("chrome://extensions/?id=pehaalcefcjfccdpbckoablngfkfgfgj");
                        _driver.ExecuteScript(
                            "document.querySelector('extensions-manager').shadowRoot.querySelector('#viewManager > extensions-detail-view.active').shadowRoot.querySelector('div#container.page-container > div.page-content > div#options-section extensions-toggle-row#allow-incognito').shadowRoot.querySelector('label#label input').click()");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now:G} -> Hata: {exception.Message}");
                    _driver.Manage().Cookies.DeleteAllCookies();
                    _service.Dispose();
                    _driver.Dispose();
                    button1.Enabled = true;
                    button2.Enabled = false;
                }

                button2.Enabled = true;
            });

            thread.Start();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void RestartNetwork()
        {
            new RestClient($"http://{mobileIpBox.Text}:8000").Execute(new RestRequest(Method.GET));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;

            var thread = new Thread(() =>
            {
                if (emailVerify.Checked)
                {
                    try
                    {
                        Console.WriteLine($"{DateTime.Now:G} -> Doğrulama maili bekleniyor: {_email}");

                        WaitWhile(async () =>
                        {
                            using var pop3Client = new Pop3Client();

                            await pop3Client.ConnectAsync(PopServer, _email, _password, false);

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

                        if (!string.IsNullOrEmpty(_emailVerifyLink))
                        {
                            _driver.Navigate().GoToUrl(_emailVerifyLink);
                            Console.WriteLine($"{DateTime.Now:G} -> Email doğrulandı: {_email}");
                        }
                        else
                        {
                            Console.WriteLine($"{DateTime.Now:G} -> Email doğrulanamadı: {_email}");
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"{DateTime.Now:G} -> Doğrulama Maili Gelmedi: {_email}");
                    }
                }
                
                var accountsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    UsersFileName);

                using (var file = new StreamWriter(accountsPath, true))
                {
                    file.WriteLine($"{_email};{_username};{_password}");
                }

                var cookies = _driver.Manage().Cookies.AllCookies.Select(c => new Cookie
                {
                    Domain = c.Domain,
                    Expires = c.Expiry ?? DateTime.MinValue,
                    HttpOnly = c.IsHttpOnly,
                    Name = c.Name,
                    Path = c.Path,
                    Secure = c.Secure,
                    Value = c.Value,
                }).ToList();
                
                var apis = Task.Run(() => _pinterestClient.App.BulkGetSpecialToken(createApi.Checked ? 1 : 0, cookies))
                    .Result;

                foreach (var api in apis)
                {
                    using var file = new StreamWriter(
                        Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                            ApisFileName),
                        true);

                    if (!string.IsNullOrEmpty(api.AccessToken))
                    {
                        file.WriteLine(
                            $"{_username};{api.AppId};{api.Secret};{api.AccessToken}");
                    }
                }

                Console.WriteLine($"{DateTime.Now:G} -> {apis.Count} api oluşturuldu: {_email}");

                _driver.Manage().Cookies.DeleteAllCookies();
                _service.Dispose();
                _driver.Dispose();

                try
                {
                    RestartNetwork();
                }
                catch
                {
                    // ignored
                }

                button1.Enabled = true;
            });

            thread.Start();
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