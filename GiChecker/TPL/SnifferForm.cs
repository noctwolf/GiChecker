using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GiChecker.TPL
{
    public partial class SnifferForm : Form
    {
        public SnifferForm()
        {
            InitializeComponent();
        }

        public static void SetParam(Sniffer sniffer)
        {
            using (SnifferForm sf = new SnifferForm())
            {
                sf.nudTimeout.Value = sniffer.Timeout;
                sf.nudThread.Value = sniffer.MaxDegreeOfParallelism;
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    sniffer.Timeout = (int)sf.nudTimeout.Value;
                    sniffer.MaxDegreeOfParallelism = (int)sf.nudThread.Value;
                }
            }
        }
    }
}
