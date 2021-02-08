using System;
using CefSharp;
using System.IO;
using CefSharp.WinForms;
using System.Diagnostics;
using System.Windows.Forms;
using CefSharp.SchemeHandler;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace NanoLauncher.Browser
{
    public delegate void SendHandleMessageDelegate();

    public class BrowserContext : ChromiumWebBrowser
    {
        private LauncherContext _launcher;

        static BrowserContext()
        {
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            Cef.EnableHighDPISupport();

            var settings = new CefSettings()
            {
                LogSeverity = LogSeverity.Verbose,
                BrowserSubprocessPath = Process.GetCurrentProcess().MainModule.FileName,
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Nano\\Cache")
            };

            settings.CefCommandLineArgs.Add("no-proxy-server");
            settings.CefCommandLineArgs.Add("disable-plugins-discovery");
            settings.CefCommandLineArgs.Add("disable-extensions");
            settings.CefCommandLineArgs.Add("disable-pdf-extension");
            settings.CefCommandLineArgs.Add("disable-demo-mode");
            settings.CefCommandLineArgs.Add("disable-default-apps");
            settings.CefCommandLineArgs.Add("disable-dinosaur-easter-egg");
            settings.CefCommandLineArgs.Add("disable-features", "ExtendedMouseButtons");

            //@TODO: Move UI to server-side
            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = "localfolder",
                DomainName = "nano",
                SchemeHandlerFactory = new FolderSchemeHandlerFactory(
                    rootFolder: @"ui",
                    hostName: "nano"
                )
            });

            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }

        public BrowserContext(LauncherContext launcher) : base("localfolder://nano/")
        {
            _launcher = launcher;

            Dock = DockStyle.Fill;
            DragHandler = new BrowserDragHandler();
            MenuHandler = new BrowserMenuHandler();
            IsBrowserInitializedChanged += OnBrowserInitializedChanged;

            JavascriptObjectRepository.ResolveObject += (sender, args) =>
            {
                var name = typeof(LauncherContext).Name;
                if (args.ObjectName == name)
                {
                    args.ObjectRepository.Register(name, _launcher, true, BindingOptions.DefaultBinder);
                }
            };
        }

        public SendHandleMessageDelegate SendHandleMessage { get; set; }

        private void OnBrowserInitializedChanged(object sender, EventArgs args)
        {
            if (IsBrowserInitialized)
            {
                BrowserWidgetMessageInterceptor.MessageLoop(this, (message) =>
                {
                    if (message.Msg == (int)Event.LeftButtonDown)
                    {
                        var point = new Point(message.LParam.ToInt32());
                        var dragHandler = DragHandler as BrowserDragHandler;
                        if (dragHandler != null && dragHandler.DraggableRegion.IsVisible(point))
                        {
                            Native.ReleaseCapture();
                            SendHandleMessage();
                        }
                    }
                });
            }
        }
    }
}
