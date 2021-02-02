using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NanoLauncher
{
    public partial class BrowserForm : Form
    {
        private readonly BrowserContext _browserContext;

        public BrowserForm(BrowserContext browserContext)
        {
            InitializeComponent();
            _browserContext = browserContext;
            Controls.Add(_browserContext.Browser);
        }

        public enum WindowEvent : int
        {
            MouseMove = 0x0200,
            MouseLeave = 0x02A3,
            LeftButtonDown = 0x0201,
            LeftButtonUp = 0x0202,
            NclButtonDown = 0xA1,
            Caption = 0x2
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public delegate void SendHandleMessageDelegate();

        public void SendHandleMessage()
        {
            if (InvokeRequired)
            {
                Invoke(new SendHandleMessageDelegate(SendHandleMessage));
            }
            else
            {
                SendMessage(Handle, (int)WindowEvent.NclButtonDown, (int)WindowEvent.Caption, 0);
            }
        }
    }
}
