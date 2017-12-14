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
            if (_Issuer.Length > 256) _Issuer = _Issuer.Substring(0, 256);
            IsSSL = (Issuer != null) || (Subject != null);
            IsGoogle = Issuer == "Google Internet Authority G2";
        }

        partial void OnSubjectChanged()
        {
            if (_Subject.Length > 256) _Subject = _Subject.Substring(0, 256);
            IsSSL = (Issuer != null) || (Subject != null);
        }

        partial void OnServerChanged()
        {
            Isgws = Server == "gws";
        }

        partial void OnCreated()
        {
            PropertyChanged += (sender, e) => { if (e.PropertyName != "UpdateTime") UpdateTime = DateTime.Now; };
        }

        public IPv4SSL(uint address)
            : this()
        {
            Address = address;
            A = (byte)(address >> 24);
            IP = address.ToIPAddress().ToString();
            RoundtripTime = -1;
            Location = IPv4Location.Find(IP)[0];
        }
    }
}
