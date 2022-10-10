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
using Bogus;
using Newtonsoft.Json.Linq;
using PinterestApi;
using PinterestDesktopBot.PinterestClient.Api;
using PinterestDesktopBot.PinterestClient.Api.Responses;
using PinterestDesktopBot.PinterestClient.Api.Session;
using PinterestDesktopBot.PinterestClient.Http;
using PinterestDesktopBot.PinterestClient.Models.Inputs;
using Pop3;
using RestSharp.Extensions;

namespace PinterestDesktopBot
{
    public partial class RepinForm : Form
    {
        private const string UsersFileName = "repin_accounts.txt";
        private const string MailDomain = "sanrecu.com";
        private const string PopServer = "116.203.221.31";

        public RepinForm()
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
            var repinCount = (int) repinCountBox.Value;

            var proxyList = proxyListBox.Lines;
            var accountList = accountBox.Lines;

            var selectedCountry = (countryComboBox.SelectedItem as ComboboxItem<string>)?.Value;

            Console.WriteLine($"{DateTime.Now:G} -> Hizmet başlatıldı.");

            var thread = new Thread(() =>
            {
                for (int i = 0; i < accountCount / asyncOperations; i++)
                {
                    var tasks = new List<Task<Task>>();

                    for (int j = 0; j < asyncOperations; j++)
                    {
                        var key = i * asyncOperations + j;

                        string proxy = null;

                        if (proxyList.Any())
                        {
                            proxy = proxyList[key == 0 ? 0 : key % proxyList.Length];
                        }

                        var task = Task.Factory.StartNew(async () =>
                        {
                            try
                            {
                                Console.WriteLine($"{DateTime.Now:G} -> Hesap oluşturuluyor...");

                                var pinterestClient = new PinterestClient.PinterestClient(string.Empty, string.Empty,
                                    false, null, proxy != null
                                        ? new ProxyData()
                                        {
                                            Address = new Uri("http://" + proxy)
                                        }
                                        : null);

                                var registeredAccount = await pinterestClient.Auth.AutoRegisterPersonalToBusinessAsync(
                                    MailDomain,
                                    selectedCountry);

                                Console.WriteLine($"{DateTime.Now:G} -> Hesap oluşturuldu: {registeredAccount.Email}");

                                pinterestClient.Common.SetUser(new UserSessionData
                                {
                                    Username = registeredAccount.Email,
                                    Password = registeredAccount.Password
                                }, true);

                                pinterestClient.Common.SetProxyData(null);

                                if (emailVerify.Checked)
                                {
                                    await pinterestClient.Auth.ResendValidationEmailAsync(false);

                                    Console.WriteLine(
                                        $"{DateTime.Now:G} -> Doğrulama maili bekleniyor: {registeredAccount.Email}");

                                    Task.Delay(TimeSpan.FromSeconds(15)).Wait();

                                    try
                                    {
                                        WaitWhile(async () =>
                                        {
                                            using (var pop3Client = new Pop3Client())
                                            {
                                                await pop3Client.ConnectAsync(PopServer, registeredAccount.Email,
                                                    registeredAccount.Password, false);

                                                var confirmMail =
                                                    (await pop3Client.ListAndRetrieveHeaderAsync()).FirstOrDefault(
                                                        message =>
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

                                                await pinterestClient.Auth.ConfirmEmailAsync(link);

                                                return false;
                                            }
                                        }, 15000, 180000);
                                    }
                                    catch
                                    {
                                        Console.WriteLine(
                                            $"{DateTime.Now:G} -> Doğrulama Maili Gelmedi: {registeredAccount.Email}");
                                        throw;
                                    }

                                    Console.WriteLine(
                                        $"{DateTime.Now:G} -> Email doğrulandı: {registeredAccount.Email}");
                                }


                                try
                                {
                                    foreach (var account in accountList)
                                    {
                                        string boardId;
                                        var boardName = new Faker().Random.Word()
                                            .ToPascalCase(true, CultureInfo.InvariantCulture);

                                        var board = (await pinterestClient.LookUp.GetBoards())
                                            .FirstOrDefault(b => b.Name == boardName);

                                        if (board != null)
                                        {
                                            boardId = board.Id;
                                        }
                                        else
                                        {
                                            var createdBoard = await pinterestClient.Boards.CreateAsync(new BoardInput()
                                            {
                                                Name = boardName
                                            });

                                            boardId = createdBoard.Value<string>("id");
                                        }

                                        var getClient = new PinterestClient.PinterestClient(
                                            PinterestApiConstants.DefaultPinterestEmail,
                                            PinterestApiConstants.DefaultPinterestPassword);

                                        var pins = new[] {new {PinId = "", Title = "", Desc = "", Link = ""}}.ToList();
                                        var totalRepin = 0;

                                        while ((pins = (await getClient.Pinners.GetPinsAsync(account))
                                            .Where(p => p.Value<string>("type") == "pin")
                                            .Where(p => p.Value<bool>("is_repin") == false)
                                            .Select(p => new
                                            {
                                                PinId = p.Value<string>("id"),
                                                Title = !string.IsNullOrEmpty(p.Value<string>("grid_title"))
                                                    ? p.Value<string>("grid_title")
                                                    : p.Value<string>("title"),
                                                Desc = !string.IsNullOrEmpty(p.Value<string>("grid_description"))
                                                    ? p.Value<string>("grid_description")
                                                    : p.Value<string>("description"),
                                                Link = p.Value<string>("link"),
                                            })
                                            .ToList()).Count > 0 && totalRepin <= repinCount)
                                        {
                                            totalRepin += pins.Count;

                                            foreach (var pin in pins)
                                            {
                                                await pinterestClient.Pins.RepinAsync(new RepinInput
                                                {
                                                    PinId = pin.PinId,
                                                    BoardId = boardId,
                                                    Title = pin.Title,
                                                    Description = pin.Desc,
                                                    Link = pin.Link
                                                });
                                            }
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Console.WriteLine(exception);
                                }

                                var accountsPath = Path.Combine(
                                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                    UsersFileName);

                                using (var file = new StreamWriter(accountsPath, true))
                                {
                                    file.WriteLine(
                                        $"{registeredAccount.Email};{registeredAccount.UserName};{registeredAccount.Password}");
                                }

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