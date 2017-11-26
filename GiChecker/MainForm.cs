﻿using System;
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
        CancellationTokenSource cts;
        Task task;

        public MainForm()
        {
            InitializeComponent();
            IPv4DataContext db = new IPv4DataContext();
            IPNetworkSet.IPv4Assigned.Add(string.Join(Environment.NewLine, db.IPv4Assigned.Select(f => f.IPBlock)));
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
                        where item.Isgxs && item.Server == "gws"
                        select item.Address;
                //var list = q.ToList().Select(f => (uint)f >> 8).Distinct().OrderBy(f => f).Select(f => string.Format("{0}/24", (f << 8).ToIPAddress()));
                var list = from item in q.ToList()
                           orderby item
                           group item by string.Format("{0}/24", ((uint)item & 0xFFFFFF00).ToIPAddress());
                //q.ToList().Select(f => (uint)f >> 8).Distinct().OrderBy(f => f).Select(f => string.Format("{0}/24", (f << 8).ToIPAddress()));
                string filename = string.Format("{0}{1}.txt", Properties.Settings.Default.MMFPath, Path.GetFileNameWithoutExtension(Application.ExecutablePath));
                SaveFile(filename, list.Select(f => f.Key));
                IEnumerable<string> publicList = "".Split(' ');
                filename = string.Format("{0}{1}", Properties.Settings.Default.MMFPath, Properties.Settings.Default.GoogleIP);
                if (File.Exists(filename)) publicList = publicList.Union(File.ReadAllLines(filename));
                filename = string.Format("{0}{1}", Properties.Settings.Default.MMFPath, Properties.Settings.Default.GoogleIPHunter);
                if (File.Exists(filename)) publicList = publicList.Union(File.ReadAllLines(filename));
                if (publicList.Count() > 0)
                {
                    //list = list.Except(File.ReadAllLines(filename));
                    var p = from itemg in list
                            join itemp in publicList on itemg.Key equals itemp into g
                            from item in g.DefaultIfEmpty()
                            where string.IsNullOrWhiteSpace(item)
                            select itemg;
                    list = p.ToList();
                    filename = string.Format("{0}{1}.Except.txt", Properties.Settings.Default.MMFPath, Path.GetFileNameWithoutExtension(Application.ExecutablePath));
                    SaveFile(filename, list.Select(f => f.Key));

                    list = from item in list.SelectMany(f => f)
                           orderby item
                           group item by string.Format("{0}/8", ((uint)item & 0xFF000000).ToIPAddress());
                    var ip = from item in list
                             select string.Join(",", from a in item select string.Format(@"""{0}""", ((uint)a).ToIPAddress())) + ",";
                    SaveFile("gae.user.txt", ip);
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
                    Search.TcpCheck(ip);
                }
                db.SubmitChanges();
            }
        }

        private void buttonGoogleIP_Click(object sender, EventArgs e)
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
            CodeSite.SendCollection("0.255.255.255", IPv4Location.Find(IPAddress.Parse("0.255.255.255")));
            Random r = new Random();
            for (int i = 0; i < 10; i++)
            {
                int ip = r.Next(int.MinValue, int.MaxValue);
                IPAddress ipa = ((uint)ip).ToIPAddress();
                CodeSite.SendCollection(ipa.ToString(), IPv4Location.Find(ipa));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPv4SSL ip = new IPv4SSL(IPAddress.Parse("111.13.101.208").ToUInt32());
            Search.WebCheck(ip);
            CodeSite.Send("ip", ip);
            ip = new IPv4SSL(IPAddress.Parse("111.13.101.208").ToUInt32());
            Search.TcpCheck(ip);
            CodeSite.Send("ip", ip);
        }
    }
}
