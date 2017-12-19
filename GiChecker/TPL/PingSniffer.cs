using GiChecker.Database;
using GiChecker.Net;
using Raize.CodeSiteLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GiChecker.TPL
{
    class PingSniffer : Sniffer
    {
        public PingSniffer()
        {
            Timeout = 1000;
            MaxDegreeOfParallelism = 4000;
        }

        protected override bool SaveDB(IEnumerable<IPv4SSL> ipa)
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

        public Task GlobalAsync()
        {
            CodeSite.EnterMethod(this, "GlobalAsync");
            try
            {
                CheckTask();
                cts = new CancellationTokenSource();
                Task = Task.Run(() =>
                {
                    try
                    {
                        var save = SaveAsync();
                        Thread.CurrentThread.Name = "GlobalAsync";
                        IPAddress ip = IPAddress.Parse(LastProgress.Ping);
                        CodeSite.Send("LastIP", ip.ToString());
                        IPNetwork net = IPNetwork.Parse(ip.ToString(), 12);
                        uint count = (uint)net.Total / 256;
                        for (uint i = net.Network.ToUInt32() >> 8; i <= uint.MaxValue >> 8; i++)
                        {
                            if (cts.IsCancellationRequested) break;
                            uint uip = i << 8;
                            if (!IPNetworkSet.IPv4Reserved.Contains(uip) && !IPNetworkSet.IPv4Assigned.Contains(uip) ^ Properties.Settings.Default.IPv4Assigned)
                                for (uint j = 0; j < 256; j++) listIP.Add(uip + j);
                            if (i % count == count - 1)//每组
                            {
                                if (listIP.Count > 0)
                                {
                                    //CodeSite.Send("listIP.Count", listIP.Count);
                                    LastProgress.Ping = listIP.First().ToIPAddress().ToString();
                                    CodeSite.Send("StartIP", LastProgress.Ping);
                                    progressFormat = string.Format("{0}-{1},{{0,8}}/{2},新增{{1,8}}", listIP.First().ToIPAddress(), listIP.Last().ToIPAddress(), listIP.Count);
                                    ExceptDB();
                                    CheckList();
                                    listIP.Clear();
                                }
                            }
                        }
                        bcIPv4SSL.CompleteAdding();
                        save.Wait();
                    }
                    catch (Exception ex)
                    {
                        ex.SendCodeSite("GlobalAsync");
                    }
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
                    int time;
                    if ((time = TryPing(uip, Timeout)) != -1)
                    {
                        Interlocked.Increment(ref newCount);
                        bcIPv4SSL.Add(new IPv4SSL(uip) { RoundtripTime = time });
                    }
                });
            }
            catch (OperationCanceledException ex)
            {
                ex.SendCodeSite("OperationCanceledException");
            }
            CodeSite.Send("新增数据", newCount);

            if (timer != null) timer.Dispose();
        }

        void ExceptDB()
        {
            using (IPv4DataContext db = new IPv4DataContext())
            {
                var listDB = (from item in db.IPv4SSL
                              where item.Address >= listIP.First() && item.Address <= listIP.Last()
                              select item.Address).ToList().Select(f => (uint)f);
                progressCount = listIP.Count;
                listIP = listIP.Except(listDB).ToList();
                progressCount -= listIP.Count;
            }
            if (progressCount > 0) CodeSite.Send("SkipCount", progressCount);
            Shuffle();
        }
    }
}
