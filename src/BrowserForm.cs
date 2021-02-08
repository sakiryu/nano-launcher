using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace NanoLauncher
{
    public partial class BrowserForm : Form
    {
        private readonly BrowserContext _browserContext;

        public BrowserForm(BrowserContext browserContext)
        {
            InitializeComponent();
            _browserContext = browserContext;
            _browserContext.IsBrowserInitializedChanged += (sender, args) =>
            {
                if (_browserContext.IsBrowserInitialized)
                {
                    BrowserWidgetMessageInterceptor.MessageLoop(_browserContext, (message) =>
                    {
                        if (message.Msg == (int)Event.LeftButtonDown)
                        {
                            var point = new Point(message.LParam.ToInt32());
                            var dragHandler = _browserContext.DragHandler as DragHandler;
                            if (dragHandler.DraggableRegion.IsVisible(point))
                            {
                                Native.ReleaseCapture();
                                SendHandleMessage();
                            }
                        }
                    });
                }
            };

            Controls.Add(_browserContext);
        }

       public delegate void SendHandleMessageDelegate();

       public void SendHandleMessage()
       {
           if (InvokeRequired)
           {
               Invoke(new SendHandleMessageDelegate(SendHandleMessage));
           }
           else
           {
               Native.SendMessage(Handle, (int)Event.NclButtonDown, (int)Event.Caption, 0);
           }
       }
    }
}
