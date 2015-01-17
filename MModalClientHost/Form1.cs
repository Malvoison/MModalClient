using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
using System.IO;
using System.Configuration;

namespace MModalClientHost
{
    public partial class Form1 : Form
    {
        private ObjectForScripting _ofs = null;

        private MModalRtcHost _rtc = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                _ofs = new ObjectForScripting();
                _ofs.HostWindow = wbSpaHost;
                _ofs.MainWindow = this;
                //  Load configuration settings
                _ofs.AppUrl = ConfigurationManager.AppSettings["appUrl"];
                _ofs.ApiUrl = ConfigurationManager.AppSettings["apiUrl"];
                _ofs.RtcUrl = ConfigurationManager.AppSettings["rtcUrl"];

                _rtc = MModalRtcHost.GetInstance();

                _ofs.RtcClient = _rtc.RtcGroup;
                _ofs.RtcGroup = _rtc.RtcGroup;

                wbSpaHost.ObjectForScripting = _ofs;

                WebBrowserHelper.ClearCache();

                wbSpaHost.Navigate(_ofs.AppUrl);                
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private object InvokePageJsFunction(string fn, string[] parameters)
        {
            return wbSpaHost.Document.InvokeScript(fn, parameters);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void hostAlertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var result = InvokePageJsFunction("hostAlert", new string[] {"I got yer alert right here..."});
            WebBrowserHelper.ClearCache();
            var result = sendJS("shell_hostAlert('I got yer alert right here');");
            Trace.WriteLine(result);
        }

        private object sendJS(string JScript)
        {
            object[] args = { JScript };
            return wbSpaHost.Document.InvokeScript("eval", args);
        }

        private void setHostMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = sendJS("setHostMessage('I do not like green eggs and ham.');");

            var result2 = sendJS("getHostMessage();");

            int x = 0;
        }

        private void initializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _ofs.InitializeSpeechMike();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void deinitializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _ofs.DeinitializeSpeechMike();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void setFileBlobToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.InitialDirectory = Environment.CurrentDirectory;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                {
                    byte[] arrFile = new byte[fs.Length];
                    fs.Read(arrFile, 0, (int)fs.Length);

                    _ofs.SetFileBlob(arrFile);
                }
            }
            
        }
    }
}
