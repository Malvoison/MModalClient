using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CefSharp.Example;
using System.Threading;
using System.Diagnostics;

namespace ClientHostCef
{
    public partial class App : Application
    {
        private Mutex mutex = new Mutex(false, "MtxClientHostCef");

        private App()
        {
            CefExample.Init();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!mutex.WaitOne(1000, false))
                this.Shutdown(-1);

            this.Properties.Add("AppUrl", ConfigurationManager.AppSettings["appUrl"]);
            this.Properties.Add("ApiUrl", ConfigurationManager.AppSettings["apiUrl"]);
            this.Properties.Add("RtcUrl", ConfigurationManager.AppSettings["rtcUrl"]);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (e.ApplicationExitCode == 0)
            {
                mutex.ReleaseMutex();
                mutex.Close();
            }
            else
            {
                Trace.WriteLine("Another instance of ClientHostCef is already running.");
            }

        }
    }
}
