using CefSharp.BrowserSubprocess;
using CefSharp.Internals;
using CefSharp.RenderProcess;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

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
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                var builder = GetHostBuilder().Build();

                using (var serviceScope = builder.Services.CreateScope())
                {
                    var form = serviceScope.ServiceProvider.GetRequiredService<BrowserForm>();
                    Application.Run(form);
                }
            }
            else
            {
                NanoSubProcess(args);
            }

        }

        private static IHostBuilder GetHostBuilder() =>
            Host.CreateDefaultBuilder().ConfigureServices(service =>
            {
                //@TODO: Create interfaces for this so DI can be useful
                service.AddScoped<BrowserForm>();
                service.AddSingleton<BrowserContext>();
                service.AddSingleton<LauncherContext>();
            });

        private static int NanoSubProcess(string[] args)
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
