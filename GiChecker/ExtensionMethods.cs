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
        public static void SendCodeSite(this Exception value, string msg)
        {
            try
            {
                CodeSite.Send(CodeSiteMsgType.Exception, msg, value);
                CodeSite.SendIf(value.StackTrace != null, CodeSiteMsgType.Exception, msg + ".StackTrace", value.StackTrace);
                if (value.InnerException != null) value.InnerException.SendCodeSite(msg);
            }
            catch (Exception ex)
            {
                CodeSite.SendException("SendCodeSite", ex);
            }
        }
    }
}
