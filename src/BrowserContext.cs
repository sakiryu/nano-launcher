using System;
using CefSharp;
using System.IO;
using CefSharp.WinForms;
using System.Diagnostics;
using System.Windows.Forms;
using CefSharp.SchemeHandler;

namespace NanoLauncher
{
    public class BrowserContext
    {
        private LauncherContext _launcher;
        public ChromiumWebBrowser Browser { get; private set; }

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

        public BrowserContext(LauncherContext launcher)
        {
            _launcher = launcher;

            Browser = new ChromiumWebBrowser("localfolder://nano/")
            {
                Dock = DockStyle.Fill
            };

            Browser.DragHandler = new DragHandler();
            Browser.MenuHandler = new MenuHandler();

            Browser.IsBrowserInitializedChanged += (sender, args) =>
            {
                if (Browser.IsBrowserInitialized)
                {
            
                }
            };

            Browser.JavascriptObjectRepository.ResolveObject += (sender, args) =>
            {
                var name = typeof(LauncherContext).Name;
                if (args.ObjectName == name)
                {
                    args.ObjectRepository.Register(name, _launcher, true, BindingOptions.DefaultBinder);
                }
            };


            Browser.LoadingStateChanged += (object s, LoadingStateChangedEventArgs e) =>
            {
                if (!e.IsLoading)
                {
                    //Stuff...
                }
                else
                {
                    //Stuff...
                }
            };
        }
    }
}
