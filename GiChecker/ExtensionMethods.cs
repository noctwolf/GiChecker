using Raize.CodeSiteLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiChecker
{
    public static class ExtensionMethods
    {
        public static void SendCodeSite(this Exception value, string msg = null)
        {
            if (msg == null) msg = value.TargetSite.Name;
            CodeSite.Send(CodeSiteMsgType.Exception, msg, value.ToString());
        }
    }
}
