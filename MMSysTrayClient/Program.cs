using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

using System.Runtime.InteropServices;

namespace MMSysTrayClient
{
    public class DesktopParent : IWin32Window
    {
        IntPtr _handle;

        public DesktopParent(IntPtr handle)
        {
            _handle = handle;
        }

        public IntPtr Handle
        {
            get { return _handle; }
        }
    }
    static class Program
    {
        public static FormSmHost _formSmHost;
        public static DesktopParent _desktopParent;

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            WebBrowserHelper.ClearCache();

            //  Read in configuration values
            string sLocalRtcUrl = ConfigurationManager.AppSettings["localRtcBind"];

            //  Initialize the main RTC object
            MModalRtcHost rtc = MModalRtcHost.GetInstance();

            _desktopParent = new DesktopParent(GetDesktopWindow());

            FormSmHost form = new FormSmHost();
            form.ApiUrl = ConfigurationManager.AppSettings["apiUrl"];
            _formSmHost = form;            
            _formSmHost.Show();
            _formSmHost.Hide();
                                
            //  show the system tray icon
            using (ProcessIcon pi = new ProcessIcon())
            {
                pi.Display();

                Application.Run();
            }            
        }
    }
}
