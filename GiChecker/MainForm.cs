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
        readonly Search search;

        public MainForm()
        {
            InitializeComponent();
            search = new Search()
            {
                ProgressIP = new Progress<IPv4SSL>(p => bindingSource1.Add(p)),
                ProgressString = new Progress<string>(p => labelCount.Text = p)
            };
            using (IPv4DataContext db = new IPv4DataContext())
            {
                IPNetworkSet.IPv4Assigned.Add(string.Join(Environment.NewLine, db.IPv4Assigned.Select(f => f.IPBlock)));
            }
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
            if (!search.Cancel()) search.IPv4SSLAsync();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            search.Cancel();
        }

        static bool SaveFile(string filename, IEnumerable<string> list)
        {
            FileInfo fi = new FileInfo(filename);
            if (!fi.Directory.Exists)
            {
                CodeSite.Send("目录不存在", fi.DirectoryName);
                return false;
            }
            if (fi.Exists)
            {
                var old = File.ReadAllLines(fi.FullName).AsEnumerable();
                if (list.SequenceEqual(old))
                {
                    CodeSite.SendNote("内容无变化{0}", fi.FullName);
                    return false;
                }
                else
                    CodeSite.SendCollection("新增", list.Except(old));
            }
            File.WriteAllLines(fi.FullName, list);
            CodeSite.SendCollection(fi.FullName, list);
            return true;
        }

        private void buttonMMF_Click(object sender, EventArgs e)
        {
            using (IPv4DataContext db = new IPv4DataContext())
            {
                var q = from item in db.IPv4SSL
                        where item.Isgws
                        select item.Address;
                q = q.Union(db.gws.Select(f => f.Address));
                var list = from item in q.ToList()
                           orderby item
                           group item by string.Format("{0}/24", ((uint)item & 0xFFFFFF00).ToIPAddress());
                string filename = string.Format("{0}{1}.txt", Properties.Settings.Default.MMFPath, Path.GetFileNameWithoutExtension(Application.ExecutablePath));
                SaveFile(filename, list.Select(f => f.Key));
                var publicList = db.GoogleIPDuan.Select(f => f.IPBlock).Union(db.GoogleIPHunter.Select(f => f.IPBlock));
                if (publicList.Count() > 0)
                {
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
                             select "\t\t\t" + string.Join(",", from a in item select string.Format(@"""{0}""", ((uint)a).ToIPAddress())) + ",";
                    SaveFile("gae.user.txt", ip);
                }
            }
        }

        private void buttonServernull_Click(object sender, EventArgs e)
        {
            using (IPv4DataContext db = new IPv4DataContext())
            {
                foreach (var ip in db.IPv4SSL.Where(f => f.IsGoogle && f.Server == null).OrderBy(f => f.Address))
                {
                    CodeSite.Send("IP", ip.IP);
                    Search.WebCheck(ip);
                    Search.TcpCheck(ip);
                    db.SubmitChanges();
                }
            }
        }

        private void buttonGoogleIP_Click(object sender, EventArgs e)
        {
            var sa = File.ReadAllLines("google ip duan.txt");
            using (IPv4DataContext db = new IPv4DataContext())
            {
                var list = db.GoogleIPDuan.ToList().Select(f => (uint)f.Address);
                foreach (var s in sa)
                {
                    try
                    {
                        IPNetwork network = IPNetwork.Parse(s);
                        if (network.ToString() == s)
                        {
                            uint address = network.Network.ToUInt32();
                            if (!list.Contains(address))
                            {
                                db.GoogleIPDuan.InsertOnSubmit(new GoogleIPDuan() { Address = address, IPBlock = s });
                                CodeSite.SendNote("新增 = {0}", s);
                            }
                        }
                        else CodeSite.SendError(s);
                    }
                    catch (Exception ex)
                    {
                        CodeSite.SendException(s, ex);
                    }
                }
                db.SubmitChanges();
            }
        }

        private void buttonGoogleIPHunter_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles(@"G:\DxgWork\GitHub\GoogleIPHunter\trunk", "*.txt", SearchOption.AllDirectories);
            List<IPNetwork> all = new List<IPNetwork>();
            using (IPv4DataContext db = new IPv4DataContext())
            {
                foreach (var file in files)
                {
                    foreach (var s in File.ReadAllLines(file))
                    {
                        try
                        {
                            IPNetwork network = IPNetwork.Parse(s);
                            if (network.ToString() == s)
                            {
                                uint address = network.Network.ToUInt32();
                                if (!db.GoogleIPHunter.Any(f => f.Address == (long)address))
                                {
                                    db.GoogleIPHunter.InsertOnSubmit(new GoogleIPHunter() { Address = address, IPBlock = s });
                                    db.SubmitChanges();
                                    CodeSite.SendNote("新增 = {0}", s);
                                }
                            }
                            else CodeSite.SendError(s);
                        }
                        catch
                        {
                            //CodeSite.SendException(s, ex);
                        }
                    }
                }
            }
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

        private void buttongws_Click(object sender, EventArgs e)
        {
            if (!search.Cancel()) search.gwsAsync();
        }
    }
}
