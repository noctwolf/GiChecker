using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using GiChecker.Net;
using Raize.CodeSiteLogging;
using System.Threading;

namespace GiChecker.TPL
{
    abstract class Sniffer
    {
        protected CancellationTokenSource cts;
        public Task Task { get; private set; }
        public int Timeout { get; set; }
        public int MaxDegreeOfParallelism { get; set; }

        protected static int TryPing(uint uip, int timeout = 1000)
        {
            using (Ping ping = new Ping())
            {
                PingReply pr = ping.Send(uip.ToIPAddress(), timeout);
                return pr.Status == IPStatus.Success ? (int)pr.RoundtripTime : -1;
            }
        }

        protected void CheckTask()
        {
            if (cts != null && !cts.IsCancellationRequested)
                throw new Exception("有未完成的任务，请先取消任务");
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

        public static string GetLocalIP()
        {
            var listNI = NetworkInterface.GetAllNetworkInterfaces();

            return "";
        }

        public Task TestAsync()
        {
            CheckTask();
            cts = new CancellationTokenSource();
            Task = Task.Run(() =>
            {

            }, cts.Token);
            return Task;
        }
    }
}
