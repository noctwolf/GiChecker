using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiChecker.TPL
{
    class PingSniffer : Sniffer
    {
        public PingSniffer()
        {
            Timeout = 1000;
            MaxDegreeOfParallelism = 10000;
        }
    }
}
