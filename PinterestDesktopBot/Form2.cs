using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PinterestApi;
using Pop3;
using RestSharp;

namespace PinterestDesktopBot
{
    public partial class Form2 : Form
    {
        private const string UsersFileName = "accounts.txt";
        private const string ApisFileName = "apis.txt";
        private const string LogsFileName = "logs.txt";
        private const string IpsFileName = "ips.txt";
        private const string MailDomain = "sanrecu.com";
        private const string PopServer = "116.203.221.31";

        private readonly PinterestClient.PinterestClient _pinterestClient;

        public Form2()
        {
            _pinterestClient = new PinterestClient.PinterestClient(string.Empty, string.Empty);
            InitializeComponent();
            Console.SetOut(new RichTextBoxWriter(richTextBox1));

            var countries = GetCoutries();
            countries.ForEach(c => countryComboBox.Items.Add(c));
            countryComboBox.SelectedItem = countries.FirstOrDefault(c => c.Value == "US");
        }

        private List<ComboboxItem<string>> GetCoutries()
        {
            var countryLookup = new List<ComboboxItem<string>>();

            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (var culture in cultures)
            {
                RegionInfo region;

                try
                {
                    region = new RegionInfo(culture.LCID);
                }
                catch (Exception e)
                {
                    continue;
                }

                if (region.Name == "001") continue;

                if (countryLookup.All(dto => dto.Value != region.TwoLetterISORegionName))
                    countryLookup.Add(new ComboboxItem<string>
                    {
                        Text = region.EnglishName,
                        Value = region.TwoLetterISORegionName.ToUpperInvariant()
                    });
            }

            return countryLookup.OrderBy(c => c.Text).ToList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            var accountCount = (int) accountCountBox.Value;

            Console.WriteLine($"{DateTime.Now:G} -> Hizmet başlatıldı.");

            var thread = new Thread(() =>
            {
                for (int i = 0; i < accountCount; i++)
                {
                    try
                    {
                        Console.WriteLine($"{DateTime.Now:G} -> Hesap oluşturuluyor...");

                        var pinterestBot = new PinterestClient.PinterestClient("", "");

                        var registeredAccount = pinterestBot.Auth.AutoRegisterPersonalToBusinessAsync(MailDomain,
                            (countryComboBox.SelectedItem as ComboboxItem<string>)?.Value).Result;

                        Console.WriteLine($"{DateTime.Now:G} -> Hesap oluşturuldu: {registeredAccount.Email}");

                        if (emailVerify.Checked)
                        {
                            pinterestBot.Auth.ResendValidationEmailAsync(false).Wait();

                            Console.WriteLine(
                                $"{DateTime.Now:G} -> Doğrulama maili bekleniyor: {registeredAccount.Email}");

                            Task.Delay(TimeSpan.FromSeconds(30)).Wait();

                            try
                            {
                                WaitWhile(async () =>
                                {
                                    using (var pop3Client = new Pop3Client())
                                    {
                                        await pop3Client.ConnectAsync(PopServer, registeredAccount.Email,
                                            registeredAccount.Password, false);

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
                                                "(https://post.pinterest.com/(.*)(autologin)(.*))$",
                                                RegexOptions.Multiline)
                                            .Select(m => m.Groups[0].Value)
                                            .FirstOrDefault(s => s.Contains("autologin"));

                                        if (link == null)
                                        {
                                            return true;
                                        }

                                        await pinterestBot.Auth.ConfirmEmailAsync(link);

                                        return false;
                                    }
                                }, 60000, 300000);
                            }
                            catch
                            {
                                Console.WriteLine(
                                    $"{DateTime.Now:G} -> Doğrulama Maili Gelmedi: {registeredAccount.Email}");
                                throw;
                            }

                            Console.WriteLine($"{DateTime.Now:G} -> Email doğrulandı: {registeredAccount.Email}");
                        }

                        var accountsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                            UsersFileName);

                        using (var file = new StreamWriter(accountsPath, true))
                        {
                            file.WriteLine(
                                $"{registeredAccount.Email};{registeredAccount.UserName};{registeredAccount.Password}");
                        }

                        var pinterestApiBot =
                            new PinterestClient.PinterestClient(registeredAccount.Email, registeredAccount.Password);

                        var apis = pinterestApiBot.App.BulkGetSpecialToken(createApi.Checked ? 1 : 0, null).Result;

                        foreach (var api in apis)
                        {
                            using var file = new StreamWriter(
                                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                    ApisFileName),
                                true);

                            if (!string.IsNullOrEmpty(api.AccessToken))
                            {
                                file.WriteLine(
                                    $"{registeredAccount.UserName};{api.AppId};{api.Secret};{api.AccessToken}");
                            }
                        }

                        Console.WriteLine(
                            $"{DateTime.Now:G} -> {apis.Count} api oluşturuldu: {registeredAccount.Email}");

                        var apiToken = apis.FirstOrDefault()?.AccessToken;

                        if (apiToken != null && !string.IsNullOrEmpty(panoBox.Text))
                        {
                            var apiClient = new PinterestApiV3Client(apiToken);

                            Task.Run(() => apiClient.Boards.CreateBoardAsync(panoBox.Text)).Wait();

                            Console.WriteLine($"{DateTime.Now:G} -> Pano açıldı: {panoBox.Text}");
                        }

                        var shadowCheckClient = new PinterestClient.PinterestClient(string.Empty, string.Empty);

                        var userInfo = Task.Run(() =>
                            shadowCheckClient.Pinners.GetUserInfoAsync(registeredAccount.UserName)).Result;

                        Console.WriteLine(
                            $"{DateTime.Now:G} -> {registeredAccount.UserName} Shadow Var Mı?: {(userInfo.Indexed ? "Hayır" : "Evet")}");

                        Console.WriteLine($"{DateTime.Now:G} -> Kalan hesap: {accountCount - (i + 1)}");
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine($"{DateTime.Now:G} -> Hata: {exception.Message}");
                    }

                    RestartNetwork();
                }

                button1.Enabled = true;
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