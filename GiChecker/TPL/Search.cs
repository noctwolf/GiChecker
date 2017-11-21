using GiChecker.Database;
using GiChecker.Net;
using Raize.CodeSiteLogging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GiChecker.TPL
{
    class Search
    {
        readonly SortedSet<uint> IPSet = new SortedSet<uint>();
        readonly List<uint> IPList = new List<uint>();
        readonly ConcurrentStack<IPv4SSL> IPStack = new ConcurrentStack<IPv4SSL>();
        readonly CancellationToken ctWeb;
        readonly IProgress<IPv4SSL> ProgressIP;
        readonly IProgress<string> ProgressString;
        CancellationTokenSource ctsSave;
        Task taskSave;
        int finishCount;
        int saveCount;
        string progressFormat;

        Search(IProgress<IPv4SSL> progressIP = null, Progress<string> progressString = null)
            : this(CancellationToken.None, progressIP, progressString)
        {

        }

        Search(CancellationToken cancellationToken, IProgress<IPv4SSL> progressIP = null, Progress<string> progressString = null)
        {
            ctWeb = cancellationToken;
            ProgressIP = progressIP;
            ProgressString = progressString;
        }

        private void SaveDB(IEnumerable<IPv4SSL> ipa)
        {
            try
            {
                if (ProgressIP != null) foreach (var item in ipa) if (item.Isgxs) ProgressIP.Report(item);
                using (IPv4DataContext db = new IPv4DataContext())
                {
                    db.IPv4SSL.InsertAllOnSubmit(ipa);
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                CodeSite.SendException("SaveDB", ex);
            }
        }

        void SaveThreadStart()
        {
            CodeSite.EnterMethod(this, "SaveThreadStart");
            try
            {
                ctsSave = new CancellationTokenSource();
                taskSave = Task.Factory.StartNew(() =>
                {
                    IPv4SSL[] ipa = new IPv4SSL[1000];
                    int c;
                    while (!ctsSave.IsCancellationRequested || !IPStack.IsEmpty)
                    {
                        if ((c = IPStack.TryPopRange(ipa)) > 0) SaveDB(ipa.Take(c));
                        if (IPStack.IsEmpty) Thread.Sleep(1000);
                    }
                }, ctsSave.Token);
            }
            catch (Exception ex)
            {
                CodeSite.SendException("SaveThreadStart", ex);
                throw;
            }
            finally
            {
                CodeSite.ExitMethod(this, "SaveThreadStart");
            }
        }

        void SaveThreadStop()
        {
            CodeSite.EnterMethod(this, "SaveThreadStop");
            try
            {
                ctsSave.Cancel();
                taskSave.Wait();
            }
            catch (Exception ex)
            {
                CodeSite.SendException("SaveThreadStop", ex);
                throw;
            }
            finally
            {
                CodeSite.ExitMethod(this, "SaveThreadStop");
            }
        }

        public static void TcpCheck(IPv4SSL ip)
        {
            IPAddress value = IPAddress.Parse(ip.IP);
            TcpClient tcpClient = new TcpClient { ReceiveTimeout = 1000, SendTimeout = 1000 };
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply pingReply = ping.Send(value, 1000);
                    if (pingReply.Status == IPStatus.Success)
                    {
                        tcpClient.Connect(value, 443);
                        SslStream sslStream = new SslStream(tcpClient.GetStream(), false,
                            (sender, certificate, chain, sslPolicyErrors) =>
                                Encoding.UTF8.GetString(certificate.GetRawCertData()).IndexOf("google") != -1);
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
            }
            catch (Exception ex)
            {
                CodeSite.SendException("TcpCheck", ex);
            }
            tcpClient.Close();
        }

        public static void WebCheck(IPv4SSL ip)
        {
            IPAddress value = IPAddress.Parse(ip.IP);
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply pr = ping.Send(value, 1000);
                    if (pr.Status == IPStatus.Success)
                    {
                        ip.RoundtripTime = (int)pr.RoundtripTime;
                        //ip = new IPv4SSL(value.ToUInt32(), pr.RoundtripTime);
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("https://{0}", value));
                        request.Timeout = 5000;
                        request.AllowAutoRedirect = false;
                        request.AllowWriteStreamBuffering = false;
                        request.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                        {
                            chain.Dispose();
                            if (certificate == null) return false;
                            ip.Issuer = ((X509Certificate2)certificate).GetNameInfo(X509NameType.SimpleName, true);
                            ip.Subject = ((X509Certificate2)certificate).GetNameInfo(X509NameType.SimpleName, false);
                            if (ip.IsGoogle) CodeSite.Send("IP", value.ToString());
                            certificate.Dispose();
                            return ip.IsGoogle;
                        };
                        request.Method = "HEAD";
                        request.KeepAlive = false;
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            ip.Server = response.Server;
                            CodeSite.Send("response", response);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                if (ip.IsGoogle)
                {
                    HttpWebResponse response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        ip.Server = response.Server;
                        CodeSite.Send("response", response);
                    }
                    else
                        CodeSite.SendException("IsGoogle", ex);
                }
            }
            catch (Exception ex)
            {
                if (ip.IsGoogle) CodeSite.SendException("WebCheck", ex);
            }
        }

        bool WebCheck(uint value)
        {
            IPv4SSL ip = new IPv4SSL(value, -1);
            WebCheck(ip);
            if (ip.RoundtripTime != -1)
            {
                if (Properties.Settings.Default.IPv4Assigned) CodeSite.Send("IP", ip.IP);
                IPStack.Push(ip);
            }
            return ip.RoundtripTime != -1;
        }

        private void ExceptDB()
        {
            using (IPv4DataContext db = new IPv4DataContext())
            {
                var listDB = (from item in db.IPv4SSL
                              where item.Address >= IPSet.Min && item.Address <= IPSet.Max
                              select item.Address).ToList().Select(p => (uint)p);
                finishCount = IPSet.Count;
                IPSet.ExceptWith(listDB);
                finishCount -= IPSet.Count;
            }
            if (finishCount > 0) CodeSite.Send("SkipCount", finishCount);
            IPList.Clear();
            IPList.AddRange(IPSet);
            Random random = new Random();
            for (int i = 0; i < IPList.Count; i++)
            {
                int j = random.Next(i, IPList.Count);
                uint t = IPList[i];
                IPList[i] = IPList[j];
                IPList[j] = t;
            }
        }

        void TPLCheckList()
        {
            try
            {
                ParallelOptions po = new ParallelOptions() { CancellationToken = ctWeb };
                Parallel.ForEach(IPSet, po, (uip) =>
                {
                    Interlocked.Increment(ref finishCount);
                    if (WebCheck(uip)) Interlocked.Increment(ref saveCount);
                });
            }
            catch (OperationCanceledException e)
            {
                CodeSite.SendException("OperationCanceledException", e);
            }
        }

        void ThreadCheckList()
        {
            ConcurrentBag<uint> csIP = new ConcurrentBag<uint>(IPList);
            Thread[] threads = new Thread[2000];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() =>
                {
                    uint uip;
                    while (csIP.TryTake(out uip))
                    {
                        if (ctWeb.IsCancellationRequested) return;
                        Interlocked.Increment(ref finishCount);
                        if (WebCheck(uip)) Interlocked.Increment(ref saveCount);
                    }
                    if (!csIP.IsEmpty) CodeSite.Send("csIP.IsEmpty", csIP.IsEmpty);
                });
                threads[i].Priority = ThreadPriority.Lowest;
                threads[i].Start();
            }
            foreach (var item in threads) item.Join();
        }

        void CheckList()
        {
            try
            {
                Timer timer = null; ;
                if (ProgressString != null)
                    timer = new Timer(p =>
                   {
                       ProgressString.Report(string.Format(progressFormat, finishCount, saveCount));
                   }, null, 1000, 1000);
                saveCount = 0;

                ThreadCheckList();
                //TPLCheckList();

                if (timer != null) timer.Dispose();
                ctWeb.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                CodeSite.SendException("ThreadCheckList", e);
            }
        }

        Task IPv4SSLAsync()
        {
            Task task = new Task(() =>
            {
                try
                {
                    SaveThreadStart();
                    Thread.CurrentThread.Name = "Async";
                    IPAddress ip = IPAddress.Parse(Properties.Settings.Default.LastIP);
                    CodeSite.Send("LastIP", ip.ToString());
                    IPNetwork net = IPNetwork.Parse(ip.ToString(), 12);
                    uint count = (uint)net.Total / 256;
                    for (uint i = net.Network.ToUInt32() >> 8; i <= uint.MaxValue >> 8; i++)
                    {
                        if (ctWeb.IsCancellationRequested) break;
                        uint uip = i << 8;
                        if (!IPNetworkSet.IPv4Reserved.Contains(uip) && !IPNetworkSet.IPv4Assigned.Contains(uip) ^ Properties.Settings.Default.IPv4Assigned)
                            for (uint j = 0; j < 256; j++) IPSet.Add(uip + j);
                        if (i % count == count - 1)//每组
                        {
                            if (IPSet.Count > 0)
                            {
                                CodeSite.Send("MinIP", IPSet.Min.ToIPAddress().ToString());
                                progressFormat = string.Format("{0}-{1},{{0,8}}/{2},New{{1,8}}", IPSet.Min.ToIPAddress(), IPSet.Max.ToIPAddress(), IPSet.Count);
                                Properties.Settings.Default.LastIP = IPSet.Min.ToIPAddress().ToString();
                                Properties.Settings.Default.Save();
                                ExceptDB();
                                CheckList();
                                IPSet.Clear();
                            }
                        }
                    }
                    SaveThreadStop();
                }
                catch (Exception ex)
                {
                    CodeSite.SendException("IPv4SSLAsync", ex);
                }
            }, ctWeb);
            task.Start();
            return task;
        }

        //TAP
        public static Task IPv4SSLAsync(CancellationToken cancellationToken, IProgress<IPv4SSL> progressIP, Progress<string> progressString)
        {
            return new Search(cancellationToken, progressIP, progressString).IPv4SSLAsync();
        }
    }
}
