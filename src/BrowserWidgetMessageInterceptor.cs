using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NanoLauncher.Browser
{
    class BrowserWidgetMessageInterceptor : NativeWindow
    {
        private Action<Message> forwardAction;

        private BrowserWidgetMessageInterceptor(ChromiumWebBrowser browser, IntPtr chromeWidgetHostHandle, Action<Message> forwardAction)
        {
            AssignHandle(chromeWidgetHostHandle);
            browser.HandleDestroyed += BrowserHandleDestroyed;
            this.forwardAction = forwardAction;
        }

        /// <summary>
        /// Asynchronously wait for the Chromium widget window to be created for the given ChromiumWebBrowser,
        /// and when created hook into its Windows message loop.
        /// </summary>
        /// <param name="browser">The browser to intercept Windows messages for.</param>
        /// <param name="forwardAction">This action will be called whenever a Windows message is received.</param>
        internal static void MessageLoop(ChromiumWebBrowser browser, Action<Message> forwardAction)
        {
            Task.Run(() =>
            {
                try
                {
                    bool foundWidget = false;
                    while (!foundWidget)
                    {
                        browser?.Invoke((Action)(async () =>
                        {
                            IntPtr chromeWidgetHostHandle;
                            if (BrowserWidgetHandleFinder.TryFindHandle(browser, out chromeWidgetHostHandle))
                            {
                                foundWidget = true;
                                _ = new BrowserWidgetMessageInterceptor(browser, chromeWidgetHostHandle, forwardAction);
                            }
                            else
                            {
                                // Chrome hasn't yet set up its message-loop window.
                                await Task.Delay(10);
                            }
                        }));
                    }
                }
                catch
                {
                    // Errors are likely to occur if browser is disposed, and no good way to check from another thread
                }
            });
        }

        private void BrowserHandleDestroyed(object sender, EventArgs e)
        {
            ReleaseHandle();

            var browser = sender as Control;
            browser.HandleDestroyed -= BrowserHandleDestroyed;
            forwardAction = null;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            forwardAction?.Invoke(m);
        }
    }

    class BrowserWidgetHandleFinder
    {
        private class ClassDetails
        {
            public IntPtr DescendantFound { get; set; }
        }

        private static bool EnumWindow(IntPtr hWnd, IntPtr lParam)
        {
            var buffer = new StringBuilder(128);
            Native.GetClassName(hWnd, buffer, buffer.Capacity);

            if (buffer.ToString() == "Chrome_RenderWidgetHostHWND")
            {
                var gcHandle = GCHandle.FromIntPtr(lParam);
                var classDetails = gcHandle.Target as ClassDetails;
                classDetails.DescendantFound = hWnd;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Chrome's message-loop Window isn't created synchronously, so this may not find it.
        /// If so, you need to wait and try again later.
        /// </summary>
        public static bool TryFindHandle(ChromiumWebBrowser browser, out IntPtr chromeWidgetHostHandle)
        {
            var classDetails = new ClassDetails();
            var gcHandle = GCHandle.Alloc(classDetails);
            Native.EnumChildWindows(browser.Handle, new EnumWindowProc(EnumWindow), GCHandle.ToIntPtr(gcHandle));

            chromeWidgetHostHandle = classDetails.DescendantFound;
            gcHandle.Free();

            return classDetails.DescendantFound != IntPtr.Zero;
        }
    }
}
