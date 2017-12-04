using System;
using System.Linq;
using Raize.CodeSiteLogging;
using GiChecker.Net;

namespace GiChecker.Database
{
    partial class IPv4SSL
    {
        partial void OnIssuerChanged()
        {
            IsSSL = (Issuer != null) || (Subject != null);
            IsGoogle = Issuer == "Google Internet Authority G2";
        }

        partial void OnSubjectChanged()
        {
            IsSSL = (Issuer != null) || (Subject != null);
        }

        partial void OnServerChanged()
        {
            Isgws = Server == "gws";
        }

        public IPv4SSL(uint address)
        {
            Address = address;
            A = (byte)(address >> 24);
            IP = address.ToIPAddress().ToString();
            RoundtripTime = -1;
            Location = IPv4Location.Find(IP)[0];
            PropertyChanged += (sender, e) => { if (e.PropertyName != "UpdateTime") UpdateTime = DateTime.Now; };
        }
    }
}
