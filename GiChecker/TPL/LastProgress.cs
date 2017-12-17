using GiChecker.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GiChecker.Net;

namespace GiChecker.TPL
{
    static class LastProgress
    {
        public static string Ping
        {
            get
            {
                using (IPv4DataContext db = new IPv4DataContext())
                {
                    var sp = db.SnifferProgress.SingleOrDefault(f => f.IP == db.ClientNetAddress());
                    return sp != null && sp.Ping != null ? sp.Ping : 0.ToIPAddress().ToString();
                }
            }
            set
            {
                using (IPv4DataContext db = new IPv4DataContext())
                {
                    var sp = db.SnifferProgress.SingleOrDefault(f => f.IP == db.ClientNetAddress());
                    if (sp == null)
                    {
                        sp = new SnifferProgress() { IP = db.ClientNetAddress() };
                        db.SnifferProgress.InsertOnSubmit(sp);
                    }
                    sp.Ping = value;
                    db.SubmitChanges();
                }
            }
        }

        public static string Ssl
        {
            get
            {
                using (IPv4DataContext db = new IPv4DataContext())
                {
                    var sp = db.SnifferProgress.SingleOrDefault(f => f.IP == db.ClientNetAddress());
                    return sp != null && sp.Ssl != null ? sp.Ssl : 0.ToIPAddress().ToString();
                }
            }
            set
            {
                using (IPv4DataContext db = new IPv4DataContext())
                {
                    var sp = db.SnifferProgress.SingleOrDefault(f => f.IP == db.ClientNetAddress());
                    if (sp == null)
                    {
                        sp = new SnifferProgress() { IP = db.ClientNetAddress() };
                        db.SnifferProgress.InsertOnSubmit(sp);
                    }
                    sp.Ssl = value;
                    db.SubmitChanges();
                }
            }
        }

        public static uint MaxIP
        {
            get
            {
                using (IPv4DataContext db = new IPv4DataContext())
                {
                    var ip = db.IPv4SSL.OrderByDescending(f => f.Address).FirstOrDefault();
                    return ip != null ? Math.Min((uint)ip.Address, uint.MaxValue) : 0;
                }
            }
        }
    }
}
