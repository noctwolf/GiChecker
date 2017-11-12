using Raize.CodeSiteLogging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GiChecker.Upgrade
{
    public static class FromNew
    {
        public static void Upgrade()
        {
            var exe = Application.ExecutablePath;
            FileInfo fi = new FileInfo(exe);
            FileInfo fiNew = new FileInfo(exe + ".New");
            FileInfo fiOld = new FileInfo(exe + ".Old");
            if (fiOld.Exists) fiOld.Delete();
            if (fiNew.Exists)
            {
                File.Move(fi.Name, fiOld.Name);
                File.Move(fiNew.Name, fi.Name);
                Application.Restart();
                Environment.Exit(0);
            }
        }
    }
}
