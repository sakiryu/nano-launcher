using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using NanoLauncher.Browser;

namespace NanoLauncher
{
    public partial class MainForm : Form
    {
        private readonly BrowserContext _browserContext;

        public MainForm(BrowserContext browserContext)
        {
            InitializeComponent();
            _browserContext = browserContext;
            _browserContext.SendHandleMessage = () => 
            {
                if (InvokeRequired)
                {
                    Invoke(new SendHandleMessageDelegate(_browserContext.SendHandleMessage));
                }
                else
                {
                    Native.SendMessage(Handle, (int)Event.NclButtonDown, (int)Event.Caption, 0);
                }
            };
            Controls.Add(_browserContext);
        }
    }
}
