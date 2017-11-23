using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Raize.CodeSiteLogging;

namespace GiChecker.UC
{
    public partial class UCFormAlign : UserControl
    {
        public UCFormAlign()
        {
            InitializeComponent();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            Rectangle rect = Screen.GetWorkingArea(ParentForm);
            Point p = new Point(0, 0);
            switch (rb.ImageAlign)
            {
                case ContentAlignment.BottomCenter:
                    p = new Point(0, 1);
                    break;
                case ContentAlignment.BottomLeft:
                    p = new Point(-1, 1);
                    break;
                case ContentAlignment.BottomRight:
                    p = new Point(1, 1);
                    break;
                case ContentAlignment.MiddleCenter:
                    p = new Point(0, 0);
                    break;
                case ContentAlignment.MiddleLeft:
                    p = new Point(-1, 0);
                    break;
                case ContentAlignment.MiddleRight:
                    p = new Point(1, 0);
                    break;
                case ContentAlignment.TopCenter:
                    p = new Point(0, -1);
                    break;
                case ContentAlignment.TopLeft:
                    p = new Point(-1, -1);
                    break;
                case ContentAlignment.TopRight:
                    p = new Point(1, -1);
                    break;
            }
            if (p.X < 0) p.X = rect.Left;
            else if (p.X > 0) p.X = rect.Right - ParentForm.Width;
            else p.X = (rect.Width - ParentForm.Width) / 2 + rect.Left;
            if (p.Y < 0) p.Y = rect.Top;
            else if (p.Y > 0) p.Y = rect.Bottom - ParentForm.Height;
            else p.Y = (rect.Height - ParentForm.Height) / 2 + rect.Top;
            ParentForm.Location = p;
        }
    }
}
