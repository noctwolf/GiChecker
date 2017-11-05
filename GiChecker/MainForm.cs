using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using Raize.CodeSiteLogging;
using System.IO;
using System.Net.Http;
using GiChecker.TPL;
using System.Threading;
using System.Threading.Tasks;
using GiChecker.Database;
using GiChecker.Net;

namespace GiChecker
{
    public partial class MainForm : Form
    {
        const string GoogleIssuer = "Google Internet Authority G2";
        const string SslWrite = @"HEAD / HTTP/1.1
Host:www.google.com
Connection:Close

";
        CancellationTokenSource cts;
        Task task;

        public MainForm()
        {
            InitializeComponent();
            FileInfo fi = new FileInfo("IPv4Reserved.txt");
            if (fi.Exists) IPNetworkSet.IPv4Reserved.Add(File.ReadAllText(fi.FullName, Encoding.Default));
            IPv4DB.Load("17monipdb.dat");
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            Ping ping = new Ping();
            foreach (var item in rtbIP.Text.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                CodeSite.Send("item", item);
                IPAddress ia = IPAddress.Parse(item);
                TcpClient tc = new TcpClient { ReceiveTimeout = 1000, SendTimeout = 1000 };
                try
                {
                    PingReply pr = ping.Send(ia, 50);
                    CodeSite.Send("RoundtripTime", pr.RoundtripTime);
                    if (pr.Status == IPStatus.Success)
                    {
                        tc.Connect(ia, 443);
                        SslStream ss = new SslStream(tc.GetStream(), false,
                            (object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
                            {
                                CodeSite.SendNote("X509Certificate");
                                bool bOld = Encoding.UTF8.GetString(certificate.GetRawCertData()).IndexOf("google") != -1;
                                CodeSite.Send("bOld", bOld);
                                X509Certificate2 c2 = certificate as X509Certificate2;
                                bool bNew = c2.GetNameInfo(X509NameType.SimpleName, true) == GoogleIssuer;// && c2.Verify();
                                if (bOld && !bNew)
                                {
                                    CodeSite.Send("Verify", c2.Verify());
                                    CodeSite.Send("Issuer.SimpleName", c2.GetNameInfo(X509NameType.SimpleName, true));
                                    CodeSite.Send("Subject.SimpleName", c2.GetNameInfo(X509NameType.SimpleName, false));
                                }
                                CodeSite.Send("bNew", bNew);
                                return bNew;
                            });
                        ss.AuthenticateAsClient("");
                        StreamReader sr = new StreamReader(ss);
                        StreamWriter sw = new StreamWriter(ss);
                        sw.Write(SslWrite);
                        sw.Flush();
                        string s = sr.ReadToEnd();
                        CodeSite.Send("s", s);
                    }
                }
                catch (Exception ex)
                {
                    CodeSite.SendException("ex", ex);
                }
                finally
                {
                    tc.Close();
                }
            }
        }

        private void buttonHttp_Click(object sender, EventArgs e)
        {
            HttpClient hc = new HttpClient(new WebRequestHandler()
            {
                AllowAutoRedirect = false,
                ServerCertificateValidationCallback = (object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
                {
                    CodeSite.SendNote("X509Certificate");
                    bool bOld = Encoding.UTF8.GetString(certificate.GetRawCertData()).IndexOf("google") != -1;
                    X509Certificate2 c2 = certificate as X509Certificate2;
                    bool bNew = c2.GetNameInfo(X509NameType.SimpleName, true) == GoogleIssuer;// && c2.Verify();
                    if (bOld && !bNew)
                    {
                        CodeSite.Send("Verify", c2.Verify());
                        CodeSite.Send("Issuer.SimpleName", c2.GetNameInfo(X509NameType.SimpleName, true));
                        CodeSite.Send("Subject.SimpleName", c2.GetNameInfo(X509NameType.SimpleName, false));
                    }
                    CodeSite.Send("bNew", bNew);
                    return bNew;
                }
            })
            {
                Timeout = TimeSpan.FromMilliseconds(3000),
                DefaultRequestHeaders =
                {
                    ConnectionClose = true
                }
            };
            Ping ping = new Ping();
            foreach (var item in rtbIP.Text.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    CodeSite.Send("item", item);
                    IPAddress ia = IPAddress.Parse(item);
                    PingReply pr = ping.Send(ia, 200);
                    CodeSite.Send("RoundtripTime", pr.RoundtripTime);
                    CodeSite.Send("Status", pr.Status);
                    if (pr.Status == IPStatus.Success)
                    {
                        HttpRequestMessage head = new HttpRequestMessage()
                        {
                            Method = HttpMethod.Head,
                            RequestUri = new Uri(string.Format("https://{0}", item))
                        };
                        HttpResponseMessage hrm = hc.SendAsync(head).Result;
                        CodeSite.Send("hrm", hrm);
                        head.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    CodeSite.SendException("ex", ex);
                }
            }
            hc.Dispose();
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dataGridView1.RowHeadersWidth - 4, e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dataGridView1.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void button扫描全球_Click(object sender, EventArgs e)
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
                CodeSite.SendNote("cts.Cancel();");
                task.Wait();
                task.Dispose();
                cts.Dispose();
            }
            else
            {
                cts = new CancellationTokenSource();
                task = Search.IPv4SSLAsync(cts.Token,
                    new Progress<IPv4SSL>(p => bindingSource1.Add(p)),
                    new Progress<string>(p => labelCount.Text = p));
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
                CodeSite.SendNote("cts.Cancel();");
                task.Wait();
            }
        }

        static bool SaveFile(string filename, IEnumerable<string> list)
        {
            if (File.Exists(filename))
            {
                var old = File.ReadAllLines(filename).AsEnumerable();
                if (list.SequenceEqual(old))
                {
                    CodeSite.SendNote("内容无变化{0}", filename);
                    return false;
                }
                else
                    CodeSite.SendCollection("新增", list.Except(old));
            }
            File.WriteAllLines(filename, list);
            CodeSite.SendCollection(filename, list);
            return true;
        }

        private void buttonMMF_Click(object sender, EventArgs e)
        {
            using (IPv4DataContext db = new IPv4DataContext())
            {
                var q = from item in db.IPv4SSL
                        where item.Server == "gws"
                        select item.Address;
                var list = q.ToList().Select(f => (uint)f >> 8).Distinct().OrderBy(f => f).Select(f => string.Format("{0}/24", (f << 8).ToIPAddress()));
                string filename = string.Format("{0}{1}.txt", Properties.Settings.Default.MMFPath, Path.GetFileNameWithoutExtension(Application.ExecutablePath));
                SaveFile(filename, list);
                filename = string.Format("{0}{1}", Properties.Settings.Default.MMFPath, Properties.Settings.Default.GoogleIP);
                if (File.Exists(filename))
                {
                    list = list.Except(File.ReadAllLines(filename));
                    filename = string.Format("{0}{1}", Properties.Settings.Default.MMFPath, Properties.Settings.Default.GoogleIPHunter);
                    if (File.Exists(filename))
                        list = list.Except(File.ReadAllLines(filename));
                    filename = string.Format("{0}{1}.Except.txt", Properties.Settings.Default.MMFPath, Path.GetFileNameWithoutExtension(Application.ExecutablePath));
                    SaveFile(filename, list);
                }
            }
        }

        private void button更新_Click(object sender, EventArgs e)
        {
            using (IPv4DataContext db = new IPv4DataContext())
            {
                foreach (var ip in db.IPv4SSL.Where(f => f.Issuer == "Google Internet Authority G2" && f.Server == null).OrderBy(f => f.Address))
                {
                    CodeSite.Send("IP", ip.IP);
                    Search.WebCheck(ip);
                }
                db.SubmitChanges();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (IPv4DataContext db = new IPv4DataContext())
            {
                bindingSource1.DataSource = db.IPv4SSL.Where(f => f.Isgxs);
            }
        }
    }
}
