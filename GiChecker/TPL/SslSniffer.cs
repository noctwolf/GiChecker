using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiChecker.TPL
{
    class SslSniffer : Sniffer
    {
        public SslSniffer()
        {
            Timeout = 5000;
            MaxDegreeOfParallelism = 500;
        }
    }
}
