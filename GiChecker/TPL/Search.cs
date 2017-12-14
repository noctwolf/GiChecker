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
        CancellationTokenSource cts, ctsSave;
        Task task, taskSave;
        int finishCount;
        int saveCount;
        string progressFormat;

        public IProgress<IPv4SSL> ProgressIP { get; set; }
        public IProgress<string> ProgressString { get; set; }

        public bool Cancel()
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                CodeSite.EnterMethod(this, "Cancel");
                try
                {
                    cts.Cancel();
                    CodeSite.SendNote("cts.Cancel();");
                    task.Wait();
                    return true;
                }
                finally
                {
                    CodeSite.ExitMethod(this, "Cancel");
                }
            }
            return false;
        }

        private static bool SaveDB(IEnumerable<IPv4SSL> ipa)
        {
            using (IPv4DataContext db = new IPv4DataContext())
            {
                try
                {
                    db.IPv4SSL.InsertAllOnSubmit(ipa);
                    db.SubmitChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    ex.SendCodeSite("SaveDB");
                    CodeSite.Send("ipa.First().IP", ipa.First().IP);
                }
            }
            return false;
        }

        void SaveThreadStart()
        {
            CodeSite.EnterMethod(this, "SaveThreadStart");
            try
            {
                ctsSave = new CancellationTokenSource();
                taskSave = Task.Factory.StartNew(() =>
                {
                    IPv4SSL[] ipa = new IPv4SSL[100];
                    int c;
                    while (!ctsSave.IsCancellationRequested || !IPStack.IsEmpty)
                    {
                        if ((c = IPStack.TryPopRange(ipa)) > 0)
                        {
                            var ip = ipa.Take(c);
                            if (ProgressIP != null) foreach (var item in ip) if (item.Isgws) ProgressIP.Report(item);
                            while (!SaveDB(ip) && !ctsSave.IsCancellationRequested) Thread.Sleep(1000);
                        }
                        if (IPStack.IsEmpty) Thread.Sleep(1000);
                    }
                }, ctsSave.Token);
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
                ex.SendCodeSite("TcpCheck");
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
                            if (ip.Issuer.Length > 256) { CodeSite.Send(ip.IP + ip.Issuer, ip.Issuer.Length); ip.Issuer = ip.Issuer.Substring(0, 256); }
                            if (ip.Subject.Length > 256) { CodeSite.Send(ip.IP + ip.Subject, ip.Subject.Length); ip.Subject = ip.Subject.Substring(0, 256); }
                            if (ip.IsGoogle) CodeSite.Send("IP", value.ToString());
                            certificate.Dispose();
                            return ip.IsGoogle;
                        };
                        request.Method = "HEAD";
                        request.KeepAlive = false;
                        try
                        {
                            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                            {
                                ip.Server = response.Server;
                                CodeSite.Send("response", response);
                            }
                        }
                        finally
                        {
                            request.Abort();
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
                        ex.SendCodeSite("IsGoogle");
                }
            }
            catch (Exception ex)
            {
                if (ip.IsGoogle) ex.SendCodeSite("WebCheck");
            }
        }

        bool WebCheck(uint value)
        {
            IPv4SSL ip = new IPv4SSL(value);
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
                ParallelOptions po = new ParallelOptions() { CancellationToken = cts.Token };
                Parallel.ForEach(IPSet, po, (uip) =>
                {
                    Interlocked.Increment(ref finishCount);
                    if (WebCheck(uip)) Interlocked.Increment(ref saveCount);
                });
            }
            catch (OperationCanceledException ex)
            {
                ex.SendCodeSite("OperationCanceledException");
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
                        if (cts.IsCancellationRequested) return;
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
                Timer timer; ;
                if (ProgressString != null)
                    timer = new Timer(p =>
                    {
                        ProgressString.Report(string.Format(progressFormat, finishCount, saveCount));
                    }, null, 1000, 1000);
                else
                    timer = null;
                saveCount = 0;

                ThreadCheckList();
                //TPLCheckList();

                if (timer != null) timer.Dispose();
                cts.Token.ThrowIfCancellationRequested();
            }
            catch (Exception ex)
            {
                ex.SendCodeSite("CheckList");
            }
        }

        public Task IPv4SSLAsync()
        {
            cts = new CancellationTokenSource();
            task = new Task(() =>
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
                        if (cts.IsCancellationRequested) break;
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
                    ex.SendCodeSite("IPv4SSLAsync");
                }
            }, cts.Token);
            task.Start();
            return task;
        }

        public Task gwsAsync()
        {
            cts = new CancellationTokenSource();
            task = Task.Factory.StartNew(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    try
                    {
                        using (IPv4DataContext db = new IPv4DataContext())
                        {
                            var q = db.gws_Old.Select(f => f.Address).Except(db.gws.Select(f => f.Address)).ToList();
                            for (int i = 0; i < q.Count - 1; i++)
                            {
                                if (cts.IsCancellationRequested) break;
                                var gws = q[i];
                                //CodeSite.Send("gws.IP", gws.IP);
                                if (ProgressString != null) ProgressString.Report(string.Format("{0}/{1}", i + 1, q.Count));
                                var ip = db.IPv4SSL.SingleOrDefault(f => f.Address == gws);
                                if (ip == null) ip = new IPv4SSL((UInt32)gws);
                                else if (ip.Isgws)
                                {
                                    //CodeSite.SendNote("跳过");
                                    continue;
                                }
                                WebCheck(ip);
                                if (ip.RoundtripTime != -1 && db.IPv4SSL.SingleOrDefault(f => f.Address == gws) == null)
                                {
                                    db.IPv4SSL.InsertOnSubmit(ip);
                                }
                                if (ip.Isgws)
                                {
                                    if (ProgressIP != null) ProgressIP.Report(ip);
                                    db.SubmitChanges();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.SendCodeSite("gwsAsync");
                        Thread.Sleep(1000);
                    }
                }
            }, cts.Token);
            return task;
        }
    }
}
