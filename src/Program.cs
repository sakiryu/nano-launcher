using CefSharp;
using CefSharp.BrowserSubprocess;
using CefSharp.Internals;
using CefSharp.RenderProcess;
using CefSharp.SchemeHandler;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NanoLauncher
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Count() < 5)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new BrowserForm());
            }
            else
            {
                NanoSubProcess(args);
            }

        }

        public static int NanoSubProcess(string[] args)
        {
            Debug.WriteLine("BrowserSubprocess starting up with command line: " + string.Join("\n", args));

            SubProcess.EnableHighDPISupport();

            int result;


            var type = args.GetArgumentValue(CefSharpArguments.SubProcessTypeArgument);

            var parentProcessId = -1;

            // The Crashpad Handler doesn't have any HostProcessIdArgument, so we must not try to
            // parse it lest we want an ArgumentNullException.
            if (type != "crashpad-handler")
            {
                parentProcessId = int.Parse(args.GetArgumentValue(CefSharpArguments.HostProcessIdArgument));
                if (args.HasArgument(CefSharpArguments.ExitIfParentProcessClosed))
                {
                    Task.Factory.StartNew(() => AwaitParentProcessExit(parentProcessId), TaskCreationOptions.LongRunning);
                }
            }

            // Use our custom subProcess provides features like EvaluateJavascript
            if (type == "renderer")
            {
                IRenderProcessHandler handler = null;
                var wcfEnabled = args.HasArgument(CefSharpArguments.WcfEnabledArgument);
                var subProcess = wcfEnabled ? new WcfEnabledSubProcess(parentProcessId, handler, args) : new SubProcess(handler, args);

                using (subProcess)
                {
                    result = subProcess.Run();
                }
            }
            else
            {
                result = SubProcess.ExecuteProcess(args);
            }

            Debug.WriteLine("BrowserSubprocess shutting down.");

            return result;
        }

        private static async void AwaitParentProcessExit(int parentProcessId)
        {
            try
            {
                var parentProcess = Process.GetProcessById(parentProcessId);
                parentProcess.WaitForExit();
            }
            catch (Exception e)
            {
                //main process probably died already
                Debug.WriteLine(e);
            }

            await Task.Delay(1000); //wait a bit before exiting

            Debug.WriteLine("BrowserSubprocess shutting down forcibly.");

            Environment.Exit(0);
        }
    }
}
