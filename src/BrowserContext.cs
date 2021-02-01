using System;
using CefSharp;
using System.IO;
using NanoLauncher.src;
using CefSharp.WinForms;
using System.Diagnostics;
using System.Windows.Forms;
using CefSharp.SchemeHandler;

namespace NanoLauncher
{
    public class TestClass
    {
        public void Message(string t)
        {
            MessageBox.Show(t);
        }

        public void Close() => Environment.Exit(1);
    }

    public class BrowserContext : NativeWindow
    {
        private Action<Message> _forwardAction;
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

        public BrowserContext()
        {
            Browser = new ChromiumWebBrowser("localfolder://nano/")
            {
                Dock = DockStyle.Fill
            };

            Browser.DragHandler = new DragHandler();

            Browser.IsBrowserInitializedChanged += (sender, args) =>
            {
                if (Browser.IsBrowserInitialized)
                {
            
                }
            };

            Browser.JavascriptObjectRepository.ResolveObject += (sender, args) =>
            {
                if (args.ObjectName == "testClass")
                {
                    args.ObjectRepository.Register("testClass", new TestClass(), true, BindingOptions.DefaultBinder);
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
