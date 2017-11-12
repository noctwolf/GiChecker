using Raize.CodeSiteLogging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using GiChecker.Net;

namespace IPCIDR
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sa = File.ReadAllLines("google ip duan.txt");
            List<IPNetwork> netList = new List<IPNetwork>();
            foreach (var s in sa)
                try
                {
                    netList.Add(IPNetwork.Parse(s));
                }
                catch (Exception ex)
                {
                    CodeSite.SendException(s, ex);
                }
            //var netSuper = IPNetwork.Supernet(netList.ToArray());
            //var q = netSuper.OrderBy(p => IPNetwork.ToBigInteger(p.Network)).Select(p => p.ToString());
            netList.Sort();
            File.WriteAllLines("GoogleIP.txt", netList.Select(p => p.ToString()));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (certificate == null) return false;
            CodeSite.Send("Issuer", certificate.Issuer);
            CodeSite.Send("Subject", certificate.Subject);
            HttpWebRequest request = sender as HttpWebRequest;
            if (request == null) return false;
            //IPv4SSL ip = new IPv4SSL(request.Address.Host, certificate);
            //IPQueue.Enqueue(ip);
            //ProcessIPQueue();
            certificate.Dispose();
            chain.Dispose();
            return true;
        }

        bool WebCheck(IPAddress value)
        {
            CodeSite.EnterMethod(this, "WebCheck");
            try
            {
                Ping ping = new Ping();
                PingReply pr = ping.Send(value, 1000);
                if (pr.Status == IPStatus.Success)
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("https://{0}", value));
                    //WebProxy wp = new WebProxy("http://127.0.0.1:8087");
                    //request.Proxy = wp;
                    request.Timeout = 5000;
                    request.AllowAutoRedirect = false;
                    request.ServerCertificateValidationCallback = RemoteCertificateValidation;
                    request.Method = "HEAD";
                    //request.Host = "www.google.com";
                    request.KeepAlive = false;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    CodeSite.Send("response", response);
                    return true;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                if (response != null) CodeSite.Send("response", response);
            }
            catch (Exception ex)
            {
                CodeSite.SendException("WebCheck", ex);
            }
            finally
            {
                CodeSite.ExitMethod(this, "WebCheck");
            }
            return false;
        }

        // FangMomFucker.TestLib
        bool TcpCheck(IPAddress value)
        {
            CodeSite.EnterMethod(this, "TcpCheck");
            TcpClient tcpClient = new TcpClient
            {
                ReceiveTimeout = 1000,
                SendTimeout = 1000
            };
            try
            {
                PingReply pingReply = new Ping().Send(value, 1000);
                if (pingReply.Status == IPStatus.Success)
                {
                    tcpClient.Connect(value, 443);
                    SslStream sslStream = new SslStream(tcpClient.GetStream(), false,
                        (a, b, c, d) => Encoding.UTF8.GetString(b.GetRawCertData()).IndexOf("google") != -1);
                    sslStream.AuthenticateAsClient("");
                    StreamReader streamReader = new StreamReader(sslStream);
                    StreamWriter streamWriter = new StreamWriter(sslStream);
                    streamWriter.Write("HEAD / HTTP/1.1\r\nHost:www.google.com\r\nConnection:Close\r\n\r\n");
                    streamWriter.Flush();
                    string text = streamReader.ReadToEnd();
                    CodeSite.Send("text", text);
                    tcpClient.Close();
                    object[] array = new object[8];
                    if (text.IndexOf("Server: gvs 1.0") != -1)
                        array[2] = "GVS";
                    else if (text.IndexOf("Server: gws") != -1)
                        array[2] = "gws";

                    string text2 = sslStream.RemoteCertificate.Subject.Split(new char[] { ',' })[0].Substring(3);
                    array[0] = value.ToString();
                    array[1] = "_OK " + pingReply.RoundtripTime.ToString().PadLeft(4, '0');
                    array[3] = text2;
                    array[4] = "001";
                    array[5] = "";
                }

            }
            catch (Exception ex)
            {
                tcpClient.Close();
                CodeSite.SendException("TcpCheck", ex);
            }
            finally
            {
                CodeSite.ExitMethod(this, "TcpCheck");
            }
            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (IPv4DataContext db = new IPv4DataContext())
            {
                //where Issuer='Google Internet Authority G2' and Server is null
                foreach (var s in db.IPv4SSL.Where(f => f.Issuer == "Google Internet Authority G2" && f.Server == null).OrderBy(f => f.Address).Select(f => f.IP))
                {
                    CodeSite.Send("IP", s);
                    WebCheck(IPAddress.Parse(s));
                    TcpCheck(IPAddress.Parse(s));
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var s in textBox1.Text.Split("\r\n,".ToArray(), StringSplitOptions.RemoveEmptyEntries))
            {
                CodeSite.Send("IP", s);
                WebCheck(IPAddress.Parse(s));
                TcpCheck(IPAddress.Parse(s));
            }
        }

        private void buttonGoogleIPHunter_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(@"G:\DxgWork\GitHub\GoogleIPHunter\trunk", "*.txt", SearchOption.AllDirectories);
            List<IPNetwork> all = new List<IPNetwork>();
            foreach (var file in files)
            {
                foreach (var s in File.ReadAllLines(file))
                {
                    try
                    {
                        all.Add(IPNetwork.Parse(s));
                    }
                    catch (Exception ex)
                    {
                        CodeSite.SendException(s, ex);
                    }
                }
            }
            all.Sort();
            File.WriteAllLines("GoogleIPHunter.txt", all.Select(p => p.ToString()));
        }

        private void buttonIPv4DB_Click(object sender, EventArgs e)
        {
            CodeSite.SendCollection("0.255.255.255", IPv4DB.Find(IPAddress.Parse("0.255.255.255")));
            Random r = new Random();
            for (int i = 0; i < 10; i++)
            {
                int ip = r.Next(int.MinValue, int.MaxValue);
                IPAddress ipa = ((uint)ip).ToIPAddress();
                CodeSite.SendCollection(ipa.ToString(), IPv4DB.Find(ipa.ToString()));
                CodeSite.SendCollection(ipa.ToString(), IPv4DB.Find(ipa));
            }
        }
    }
}
