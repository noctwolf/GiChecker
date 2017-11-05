using System;
using System.Linq;
using Raize.CodeSiteLogging;
using GiChecker.Net;

namespace GiChecker.Database
{
    partial class IPv4SSL
    {
        const string GoogleIssuer = "Google Internet Authority G2";
        readonly string[] gxs = new string[] { "gws", "gvs 1.0" };

        public bool IsGoogle { get { return Issuer == GoogleIssuer; } }

        partial void OnAddressChanged()
        {
            IP = ((uint)Address).ToIPAddress().ToString();
            Location = IPv4DB.Find(IP)[1];
        }

        partial void OnServerChanged()
        {
            CodeSite.EnterMethod(this, "OnServerChanged");
            try
            {
                CodeSite.Send("Server", Server);
                Isgxs = gxs.Contains(Server);
            }
            catch (Exception ex)
            {
                CodeSite.SendException("OnServerChanged", ex);
                throw;
            }
            finally
            {
                CodeSite.ExitMethod(this, "OnServerChanged");
            }
        }

        public IPv4SSL(uint address, long roundtripTime)
        {
            Address = (int)address;
            RoundtripTime = (int)roundtripTime;
        }
    }
}
