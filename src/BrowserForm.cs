using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NanoLauncher
{
    public partial class BrowserForm : Form
    {
        private readonly BrowserContext bContext;
        private Rectangle rTitleBar;

        private const int WM_NCHITTEST = 0x84;
        private const int HT_CAPTION = 0x2;

        public BrowserForm()
        {
            InitializeComponent();
            rTitleBar = new Rectangle(0, 0, Width, 30);
            Height -= rTitleBar.Height;
            
            bContext = new BrowserContext();
            Controls.Add(bContext.Browser);
        }

        protected override void DefWndProc(ref Message m)
        {
            if(m.Msg == WM_NCHITTEST)
            {
                var pt = new Point(m.LParam.ToInt32());
                if(rTitleBar.Contains(pt))
                {
                    m.Result = new IntPtr(HT_CAPTION);
                }
            }
            base.DefWndProc(ref m);
        }
    }
}
