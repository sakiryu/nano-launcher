using CefSharp;
using CefSharp.BrowserSubprocess;
using CefSharp.JavascriptBinding;
using CefSharp.SchemeHandler;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace NanoLauncher
{
    public class TestClass
    {
        public void Message(string t)
        {
            MessageBox.Show(t);
        }
    }

    public class BrowserContext
    {
       // public CefSettings Settings { get; private set; }
        public ChromiumWebBrowser Browser { get; private set; }
      //  public BrowserSubprocessExecutable Subprocess { get; private set; }

        static BrowserContext()
        {
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            CefSharp.Cef.EnableHighDPISupport();

            var settings = new CefSettings();
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
                DomainName = "cefsharp",
                SchemeHandlerFactory = new FolderSchemeHandlerFactory(
                    rootFolder: @"D:\Users\Haise\source\repos\NanoLauncher\ui",
                    hostName: "cefsharp"
                )
            });

            CefSharp.Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
        }

        public BrowserContext()
        {
            //   CefSharp.Cef.EnableHighDPISupport();

            //Subprocess = new BrowserSubprocessExecutable();
            //  Subprocess.Main(new[] { string.Empty });

            // Settings = new CefSettings()
            // {
            //   //  BrowserSubprocessPath = Process.GetCurrentProcess().MainModule.FileName,
            //    // MultiThreadedMessageLoop = true
            // };
            //
            // Settings.CefCommandLineArgs.Add("no-proxy-server");
            // Settings.CefCommandLineArgs.Add("disable-plugins-discovery");
            // Settings.CefCommandLineArgs.Add("disable-extensions");
            // Settings.CefCommandLineArgs.Add("disable-pdf-extension");
            // Settings.CefCommandLineArgs.Add("disable-demo-mode");
            // Settings.CefCommandLineArgs.Add("disable-default-apps");
            // Settings.CefCommandLineArgs.Add("disable-dinosaur-easter-egg");
            // Settings.CefCommandLineArgs.Add("disable-features", "ExtendedMouseButtons");
            //
            // Settings.RegisterScheme(new CefCustomScheme
            // {
            //     SchemeName = "localfolder",
            //     DomainName = "cefsharp",
            //     SchemeHandlerFactory = new FolderSchemeHandlerFactory(
            //         rootFolder: @"D:\Users\Haise\source\repos\NanoLauncher\ui",
            //         hostName: "cefsharp"
            //     )
            // });
            //
            // CefSharp.Cef.Initialize(Settings, performDependencyCheck: true, browserProcessHandler: null);

            Browser = new ChromiumWebBrowser("localfolder://cefsharp/")
            {
                Dock = DockStyle.Fill
            };

            //Browser.JavascriptObjectRepository.Register("boundEventHandler", eventHandler, true, BindingOptions.DefaultBinder);

           // Browser.JavascriptObjectRepository.Register("testClass", new TestClass(), true, BindingOptions.DefaultBinder);

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
                   // Browser.JavascriptObjectRepository.Register("testClass", new TestClass(), true);
                    //Browser.ShowDevTools();
                    //Browser.ExecuteScriptAsync("objectTest.messageShow(\"myObject\");");
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
