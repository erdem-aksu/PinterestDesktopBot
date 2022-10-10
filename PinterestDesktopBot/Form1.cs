using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PinterestApi;
using RestSharp;

namespace PinterestDesktopBot
{
    public partial class Form1 : Form
    {
        private const string UsersFileName = "accounts.txt";
        private const string ApisFileName = "apis.txt";
        private const string LogsFileName = "logs.txt";
        private const string IpsFileName = "ips.txt";
        private const string MailDomain = "sanrecu.com";
        private const string PopServer = "116.203.221.31";

        public Form1()
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

            Console.WriteLine($"{DateTime.Now:G} -> Hizmet başlatıldı.");

            var thread = new Thread(() =>
            {
                for (int i = 0; i < accountCount / 3; i++)
                {
                    var tasks = new List<Task<Task>>();

                    for (int j = 0; j < 3; j++)
                    {
                        var task = Task.Factory.StartNew(async () =>
                        {
                            try
                            {
                                var pinterestClient = new PinterestClient.PinterestClient(string.Empty, string.Empty);

                                Console.WriteLine($"{DateTime.Now:G} -> Hesap oluşturuluyor...");

                                var registeredAccount = pinterestClient.Web.CreateAccount(null, MailDomain,
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

                                var apis = await pinterestClient.App.BulkGetSpecialToken(createApi.Checked ? 1 : 0,
                                    registeredAccount.Cookies);

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
                                    $"{DateTime.Now:G} -> Kalan hesap: {accountCount - ((i + 1) * (j + 1))}");
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine($"{DateTime.Now:G} -> Hata: {exception.Message}");
                            }
                        });

                        tasks.Add(task);
                    }

                    Task.WhenAll(Task.WhenAll(tasks).Result).Wait();

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

        private void manuelButton1_Click(object sender, EventArgs e)
        {
            var frm = new ManuelForm {Location = this.Location, StartPosition = FormStartPosition.Manual};
            frm.FormClosing += delegate
            {
                this.Show();
                Console.SetOut(new RichTextBoxWriter(this.richTextBox1));
            };
            frm.Show();
            Console.SetOut(new RichTextBoxWriter(frm.richTextBox1));
            Hide();
        }

        private void botButton1_Click(object sender, EventArgs e)
        {
            var frm = new Form2 {Location = this.Location, StartPosition = FormStartPosition.Manual};
            frm.FormClosing += delegate
            {
                this.Show();
                Console.SetOut(new RichTextBoxWriter(this.richTextBox1));
            };
            frm.Show();
            Console.SetOut(new RichTextBoxWriter(frm.richTextBox1));
            Hide();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var frm = new ProxyForm {Location = this.Location, StartPosition = FormStartPosition.Manual};
            frm.FormClosing += delegate
            {
                this.Show();
                Console.SetOut(new RichTextBoxWriter(this.richTextBox1));
            };
            frm.Show();
            Console.SetOut(new RichTextBoxWriter(frm.richTextBox1));
            Hide();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var frm = new ProxyForm2 {Location = this.Location, StartPosition = FormStartPosition.Manual};
            frm.FormClosing += delegate
            {
                this.Show();
                Console.SetOut(new RichTextBoxWriter(this.richTextBox1));
            };
            frm.Show();
            Console.SetOut(new RichTextBoxWriter(frm.richTextBox1));
            Hide();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            var frm = new RepinForm {Location = this.Location, StartPosition = FormStartPosition.Manual};
            frm.FormClosing += delegate
            {
                this.Show();
                Console.SetOut(new RichTextBoxWriter(this.richTextBox1));
            };
            frm.Show();
            Console.SetOut(new RichTextBoxWriter(frm.richTextBox1));
            Hide();
        }
    }
}