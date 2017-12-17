using GiChecker.Database;
using GiChecker.Net;
using Raize.CodeSiteLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GiChecker.TPL
{
    class SslSniffer : Sniffer
    {
        List<IPv4SSL> listIPv4SSL;

        public SslSniffer()
        {
            Timeout = 5000;
            MaxDegreeOfParallelism = 1000;
        }

        protected override bool SaveDB(IEnumerable<IPv4SSL> ipa)
        {
            //CodeSite.Send("ipa.Count()", ipa.Count());
            using (IPv4DataContext db = new IPv4DataContext())
            {
                try
                {
                    foreach (var ip in ipa)
                    {
                        if (ip.Isgws && ProgressIPv4SSL != null) ProgressIPv4SSL.Report(ip);
                        int i = listIPv4SSL.BinarySearch(ip);
                        if (i > -1)
                        {
                            //CodeSite.Send(listIPv4SSL[i].IP, listIPv4SSL[i]);
                            //CodeSite.Send(ip.IP, ip);
                            db.IPv4SSL.Attach(ip, listIPv4SSL[i]);
                        }
                        else
                            CodeSite.SendError(string.Format("{0}-{1}", i, ip.IP));
                    }
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

        public Task GlobalAsync()
        {
            CodeSite.EnterMethod(this, "GlobalAsync");
            try
            {
                CheckTask();
                cts = new CancellationTokenSource();
                Task = Task.Run(() =>
                {
                    var save = SaveAsync();
                    Thread.CurrentThread.Name = "GlobalAsync";
                    IPAddress ip = IPAddress.Parse(LastProgress.Ssl);
                    CodeSite.Send("LastIP", ip.ToString());
                    IPNetwork net = IPNetwork.Parse(ip.ToString(), 12);
                    while (net.Broadcast.ToUInt32() < LastProgress.MaxIP)
                    {
                        if (cts.IsCancellationRequested) break;
                        using (IPv4DataContext db = new IPv4DataContext())
                        {
                            listIPv4SSL = db.IPv4SSL
                                .Where(f => !f.IsSSL && f.Address >= net.Network.ToUInt32() && f.Address <= net.Broadcast.ToUInt32())
                                .OrderBy(f => f.Address)
                                .ToList();
                        }
                        listIP = listIPv4SSL.Select(f => (uint)f.Address).ToList();
                        if (listIP.Count > 0)
                        {
                            //CodeSite.Send("listIP.Count", listIP.Count);
                            LastProgress.Ssl = listIP.First().ToIPAddress().ToString();
                            CodeSite.Send("StartIP", LastProgress.Ssl);
                            progressFormat = string.Format("{0}-{1},{{0,8}}/{2},新增{{1,8}}", listIP.First().ToIPAddress(), listIP.Last().ToIPAddress(), listIP.Count);
                            Shuffle();
                            CheckList();
                            listIP.Clear();
                        }
                        if (net.Broadcast.ToUInt32() == uint.MaxValue) break;
                        net = IPNetwork.Parse((net.Broadcast.ToUInt32() + 1).ToIPAddress(), net.Netmask);
                    }
                    bcIPv4SSL.CompleteAdding();
                    save.Wait();
                    //cts.Token.ThrowIfCancellationRequested();
                }, cts.Token);
                return Task;
            }
            finally
            {
                CodeSite.ExitMethod(this, "GlobalAsync");
            }
        }

        private void CheckList()
        {
            int newCount = 0, lastCount = progressCount, tempCount;
            Timer timer;
            if (ProgressString != null)
                timer = new Timer(_ =>
                {
                    ProgressString.Report(string.Format(progressFormat, progressCount, newCount));
                    tempCount = progressCount;
                    //CodeSite.Send("progressCount", tempCount - lastCount);
                    lastCount = tempCount;
                }, null, 1000, 1000);
            else
                timer = null;

            try
            {
                ParallelOptions po = new ParallelOptions() { CancellationToken = cts.Token, MaxDegreeOfParallelism = MaxDegreeOfParallelism };
                //CodeSite.Send("po.MaxDegreeOfParallelism", po.MaxDegreeOfParallelism);
                Parallel.ForEach(listIP, po, (uip) =>
                {
                    Interlocked.Increment(ref progressCount);
                    IPv4SSL ip = new IPv4SSL(uip);
                    WebCheck(ip);
                    if (ip.IsSSL)
                    {
                        Interlocked.Increment(ref newCount);
                        bcIPv4SSL.Add(ip);
                    }
                });
            }
            catch (OperationCanceledException ex)
            {
                ex.SendCodeSite("OperationCanceledException");
            }
            while (bcIPv4SSL.Count > 0)
            {
                CodeSite.SendReminder("等待数据保存完毕");
                Thread.Sleep(1000);
            }
            CodeSite.Send("newCount", newCount);

            if (timer != null) timer.Dispose();
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
    }
}
