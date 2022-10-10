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
using PinterestDesktopBot.PinterestClient.Http;
using Pop3;
using RestSharp;

namespace PinterestDesktopBot
{
    public partial class ProxyForm2 : Form
    {
        private const string UsersFileName = "accounts.txt";
        private const string ApisFileName = "apis.txt";
        private const string LogsFileName = "logs.txt";
        private const string IpsFileName = "ips.txt";
        private const string MailDomain = "sanrecu.com";
        private const string PopServer = "116.203.221.31";

        public ProxyForm2()
        {
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
            var asyncOperations = (int) simultaneousBox.Value;

            var proxyList = proxyListBox.Lines;

            Console.WriteLine($"{DateTime.Now:G} -> Hizmet başlatıldı.");

            var thread = new Thread(() =>
            {
                for (int i = 0; i < accountCount / asyncOperations; i++)
                {
                    var tasks = new List<Task<Task>>();

                    for (int j = 0; j < asyncOperations; j++)
                    {
                        var key = i * asyncOperations + j;

                        var proxy = proxyList[key == 0 ? 0 : key % proxyList.Length];

                        Console.WriteLine($"{key} Proxy -> {proxy}");

                        var task = Task.Factory.StartNew(async () =>
                        {
                            try
                            {
                                Console.WriteLine($"{DateTime.Now:G} -> Hesap oluşturuluyor...");

                                var pinterestClient = new PinterestClient.PinterestClient(string.Empty, string.Empty,
                                    false, null, new ProxyData()
                                    {
                                        Address = new Uri("http://" + proxy)
                                    });

                                var registeredAccount = pinterestClient.Web.CreateAccount(proxy, MailDomain,
                                    emailVerify.Checked,
                                    (countryComboBox.SelectedItem as ComboboxItem<string>)?.Value);
                                
                                Console.WriteLine($"{DateTime.Now:G} -> Hesap oluşturuldu: {registeredAccount.Email}");

                                var accountsPath = Path.Combine(
                                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                    UsersFileName);

                                using (var file = new StreamWriter(accountsPath, true))
                                {
                                    file.WriteLine(
                                        $"{registeredAccount.Email};{registeredAccount.UserName};{registeredAccount.Password}");
                                }

                                var apis = await pinterestClient.App.BulkGetSpecialToken(createApi.Checked ? 1 : 0, registeredAccount.Cookies);

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
                                    var apiClient = new PinterestApiV3Client(apiToken, new PinterestApi.Http.ProxyData()
                                    {
                                        Address = new Uri("http://" + proxy)
                                    });

                                    await apiClient.Boards.CreateBoardAsync(panoBox.Text);

                                    Console.WriteLine($"{DateTime.Now:G} -> Pano açıldı: {panoBox.Text}");
                                }

                                // var shadowCheckClient = new PinterestClient.PinterestClient(string.Empty, string.Empty);
                                //
                                // var userInfo = Task.Run(() =>
                                //     shadowCheckClient.Pinners.GetUserInfoAsync(registeredAccount.UserName)).Result;
                                //
                                // Console.WriteLine(
                                //     $"{DateTime.Now:G} -> {registeredAccount.UserName} Shadow Var Mı?: {(userInfo.Indexed ? "Hayır" : "Evet")}");

                                Console.WriteLine(
                                    $"{DateTime.Now:G} -> Kalan hesap: {accountCount - key}");
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine($"{DateTime.Now:G} -> Hata: {exception.Message}");
                            }
                        });

                        tasks.Add(task);
                    }

                    Task.WhenAll(Task.WhenAll(tasks).Result).Wait();
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