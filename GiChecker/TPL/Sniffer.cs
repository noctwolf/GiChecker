using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using GiChecker.Net;
using Raize.CodeSiteLogging;
using System.Threading;
using System.Collections.Concurrent;
using GiChecker.Database;

namespace GiChecker.TPL
{
    public abstract class Sniffer
    {
        protected List<uint> listIP = new List<uint>();
        protected BlockingCollection<IPv4SSL> bcIPv4SSL;
        protected CancellationTokenSource cts;
        protected string progressFormat;
        protected int progressCount;

        public Task Task { get; protected set; }
        public int Timeout { get; set; }
        public int MaxDegreeOfParallelism { get; set; }
        public IProgress<IPv4SSL> ProgressIPv4SSL { get; set; }
        public IProgress<string> ProgressString { get; set; }

        protected static int TryPing(uint uip, int timeout = 1000)
        {
            //Thread.Sleep(1000);
            //return -1;
            using (Ping ping = new Ping())
            {
                PingReply pr = ping.Send(uip.ToIPAddress(), timeout);
                return pr.Status == IPStatus.Success ? (int)pr.RoundtripTime : -1;
            }
        }

        public static bool SetMinThreads(int threadCount)
        {
            CodeSite.EnterMethod("Sniffer." + "SetMinThreads");
            try
            {
                int workerMin, workerMax, workerA, completionPort;
                ThreadPool.GetAvailableThreads(out workerA, out completionPort);
                ThreadPool.GetMaxThreads(out workerMax, out completionPort);
                ThreadPool.GetMinThreads(out workerMin, out completionPort);
                CodeSite.Send("workerA", workerA);
                CodeSite.Send("workerMax", workerMax);
                CodeSite.Send("workerMin", workerMin);
                int worker = workerMax - workerA + threadCount + 200;
                CodeSite.Send("worker", worker);
                if (worker > workerMin)
                {
                    worker = Math.Min(worker, workerMax);
                    CodeSite.Send("worker", worker);
                    return ThreadPool.SetMinThreads(worker, completionPort);
                }
                else
                    return true;
            }
            finally
            {
                CodeSite.ExitMethod("Sniffer." + "SetMinThreads");
            }
        }

        protected void CheckTask()
        {
            CodeSite.EnterMethod(this, "CheckTask");
            try
            {
                if (cts != null && !cts.IsCancellationRequested)
                    throw new Exception("有未完成的任务，请先取消任务");
                else
                    SetMinThreads(MaxDegreeOfParallelism);
            }
            finally
            {
                CodeSite.ExitMethod(this, "CheckTask");
            }
        }

        protected abstract bool SaveDB(IEnumerable<IPv4SSL> ipa);

        protected Task SaveAsync()
        {
            CodeSite.EnterMethod(this, "SaveAsync");
            try
            {
                bcIPv4SSL = new BlockingCollection<IPv4SSL>();
                return Task.Run(() =>
                {
                    List<IPv4SSL> list = new List<IPv4SSL>();
                    IPv4SSL ip;
                    while (!cts.IsCancellationRequested || !bcIPv4SSL.IsCompleted)
                    {
                        while (list.Count < 1000)
                        {
                            if (bcIPv4SSL.TryTake(out ip))
                            {
                                list.Add(ip);
                            }
                            else
                            {
                                if (bcIPv4SSL.IsCompleted) break;
                                if (bcIPv4SSL.Count == 0) Thread.Sleep(1000);
                            }
                        }
                        if (list.Count > 0)
                        {
                            while (!SaveDB(list) && !cts.IsCancellationRequested) Thread.Sleep(1000);
                            list.Clear();
                        }
                    }
                }, cts.Token);
            }
            finally
            {
                CodeSite.ExitMethod(this, "SaveAsync");
            }
        }

        protected void Shuffle()
        {
            Random random = new Random();
            for (int i = 0; i < listIP.Count; i++)
            {
                int j = random.Next(i, listIP.Count);
                uint t = listIP[i];
                listIP[i] = listIP[j];
                listIP[j] = t;
            }
        }

        public bool Cancel(bool wait = true)
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                CodeSite.EnterMethod(this, "Cancel");
                try
                {
                    CodeSite.SendNote("cts.Cancel();");
                    cts.Cancel();
                    if (wait) Task.Wait();
                    return true;
                }
                finally
                {
                    CodeSite.ExitMethod(this, "Cancel");
                }
            }
            return false;
        }

        public Task TestAsync()
        {
            CodeSite.EnterMethod(this, "TestAsync");
            try
            {
                CheckTask();
                cts = new CancellationTokenSource();
                Task = Task.Run(() =>
                {

                }, cts.Token);
                return Task;
            }
            finally
            {
                CodeSite.ExitMethod(this, "TestAsync");
            }
        }
    }
}
