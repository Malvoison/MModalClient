using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Windows.Forms;
using MMSysTrayClient.Properties;

namespace MMSysTrayClient
{
    class ProcessIcon : IDisposable
    {
        NotifyIcon ni;

        public ProcessIcon()
        {
            ni = new NotifyIcon();
        }

        public void Display()
        {
            ni.MouseClick += ni_MouseClick;
            ni.Icon = Resources.SystemTrayApp;
            ni.Text = "MMSysTrayClient";
            ni.Visible = true;

            //  attach context menu here
            ni.ContextMenuStrip = new ContextMenus().Create();
        }

        public void Dispose()
        {
            ni.Dispose();
        }

        void ni_MouseClick(object sender, MouseEventArgs e)
        {
            //  it remains to be seen what should go here
        }

    }
}
