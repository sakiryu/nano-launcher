using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NanoLauncher.src
{
    public class BrowserWindowContext : NativeWindow
    {
        private Action<Message> _forwardAction;
        private BrowserContext _browserContext;
    }
}
