using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace NanoLauncher
{
    /*internal class BrowserWidgetMessageInterceptor : NativeWindow
    {
        private Action<Message> forwardAction;

        internal BrowserWidgetMessageInterceptor(Control browser, IntPtr chromeWidgetHostHandle, Action<Message> forwardAction)
        {
            AssignHandle(chromeWidgetHostHandle);

            browser.HandleDestroyed += BrowserHandleDestroyed;

            this.forwardAction = forwardAction;
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

    internal static class BrowserWidgetHandleFinder
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
        public static bool TryFindHandle(IntPtr browserHandle, out IntPtr chromeWidgetHostHandle)
        {
            var classDetails = new ClassDetails();
            var gcHandle = GCHandle.Alloc(classDetails);

            //var childProc = new EnumWindowProc(EnumWindow);
            Native.EnumChildWindows(browserHandle, EnumWindow, GCHandle.ToIntPtr(gcHandle));

            chromeWidgetHostHandle = classDetails.DescendantFound;
            gcHandle.Free();

            return classDetails.DescendantFound != IntPtr.Zero;
        }
    } */
}
